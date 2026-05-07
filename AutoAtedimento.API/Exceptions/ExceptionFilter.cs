using AutoAtedimento.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.SqlClient;
using System.Net;

namespace INFRA.SHARED.Filtros
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Erro capturado pelo ExceptionFilter");

            if (context.Exception is ErrorOnValidationException validationEx)
            {
                context.Result = new BadRequestObjectResult(
                    new ResponseErrorJson(validationEx.ErrosMessages)
                );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is SqlException ex)
            {
                var mensagem = ex.Number switch
                {
                    2627 or 2601 => "Registro já cadastrado.",
                    547 => "Erro de integridade de dados.",
                    _ => "Erro no banco de dados."
                };

                context.Result = new BadRequestObjectResult(
                    new ResponseErrorJson(new List<string> { mensagem })
                );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is NotFoundException notFoundEx)
            {
                context.Result = new NotFoundObjectResult(
                    new ResponseErrorJson(new List<string> { notFoundEx.Message })
                );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (context.Exception is BusinessException businessEx)
            {
                context.Result = new BadRequestObjectResult(
                    new ResponseErrorJson(new List<string> { businessEx.Message })
                );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                HandleInternalServerError(context);
            }

            context.ExceptionHandled = true;
        }

        private void HandleInternalServerError(ExceptionContext context)
        {
            context.Result = new ObjectResult(
                new ResponseErrorJson(new List<string>
                {
                    "Erro interno no servidor. Tente novamente mais tarde."
                })
            )
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}