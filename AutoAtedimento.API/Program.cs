using AutoAtedimento.API.Data;
using AutoAtedimento.API.Repositories;
using AutoAtedimento.API.Services;
using AutoAtedimento.API.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using INFRA.SHARED.Filtros;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

// 🔌 DB
builder.Services.AddScoped<DbSession>();

// 🧠 Services
builder.Services.AddScoped<PedidoRepository>();
builder.Services.AddScoped<PedidoService>();

builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<CategoriaService>();

builder.Services.AddScoped<MesaRepository>();
builder.Services.AddScoped<MesaService>();

builder.Services.AddScoped<MesaSessaoRepository>();
builder.Services.AddScoped<MesaSessaoService>();

builder.Services.AddScoped<PagamentoRepository>();
builder.Services.AddScoped<PagamentoService>();


builder.Services.AddSignalR();


// 🎯 Controllers + Filtro
builder.Services.AddScoped<ExceptionFilter>(); // 🔥 ADICIONAR ISSO

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();

});

// ✅ FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<PedidoValidator>();
builder.Services.AddFluentValidationAutoValidation();

// 📄 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

app.MapHub<PedidoHub>("/pedidoHub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSerilogRequestLogging();

// 🔐 (ativa quando tiver JWT)
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// 🔥 Finalização correta do Serilog
try
{
    Log.Information("Aplicação iniciada");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal");
}
finally
{
    Log.CloseAndFlush();
}