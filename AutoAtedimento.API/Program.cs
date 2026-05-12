using AutoAtedimento.API.Data;
using AutoAtedimento.API.Repositories;
using AutoAtedimento.API.Services;
using AutoAtedimento.API.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using INFRA.SHARED.Filtros;
using MercadoPago.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region SERILOG

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region MERCADO PAGO

MercadoPagoConfig.AccessToken =
    builder.Configuration["MercadoPago:AccessToken"];

#endregion

#region DATABASE

builder.Services.AddScoped<DbSession>();

#endregion

#region REPOSITORIES

builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<PedidoRepository>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<MesaRepository>();
builder.Services.AddScoped<MesaSessaoRepository>();
builder.Services.AddScoped<PagamentoRepository>();
#endregion

#region SERVICES

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<MesaService>();
builder.Services.AddScoped<MesaSessaoService>();
builder.Services.AddScoped<PagamentoService>();
builder.Services.AddScoped<MercadoPagoService>();

#endregion

#region SIGNALR

builder.Services.AddSignalR();

#endregion

#region FILTERS

builder.Services.AddScoped<ExceptionFilter>();

#endregion

#region JWT

var key =
    Encoding.UTF8.GetBytes(
        builder.Configuration["Jwt:Key"]!);

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],
                ValidAudience =
                    builder.Configuration["Jwt:Audience"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(key)
            };
    });

#endregion

#region CONTROLLERS

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

#endregion

#region FLUENT VALIDATION

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PedidoValidator>();

#endregion

#region API BEHAVIOR
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.SuppressModelStateInvalidFilter = true;
});

#endregion

#region SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "AutoAtendimento API",
            Version = "v1"
        });

    // 🔥 JWT AUTH
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Digite: Bearer {seu token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

#endregion
var app = builder.Build();

#region PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region SIGNALR MAP
app.MapHub<PedidoHub>("/pedidoHub");
#endregion

#region CONTROLLERS MAP
app.MapControllers();
#endregion

#region START APP
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

#endregion