using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace EmployeeManagementSystem.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {

        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService, 
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment hostEnvironment
        )
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }
     public async ValueTask<bool> TryHandleAsync
     
        (
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken

        )
       {
        var statusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        // LOG BASED ON STATUS
        
        if (statusCode >= 500)
         {
           _logger.LogError(exception, "Server error occurred");
         }
        else
         {
           _logger.LogWarning(exception, "Client error occurred");
         }
        

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
          HttpContext  =httpContext,
          Exception = exception,
          ProblemDetails =  new ProblemDetails
          {
              Status = httpContext.Response.StatusCode,
              Type = exception.GetType().Name,
              Title = exception switch
            {
               ApplicationException => "Bad Request",
               UnauthorizedAccessException => "Unauthorized",
               KeyNotFoundException => "Not Found",
               _ => "Server Error"
            },
              Detail = _hostEnvironment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred",
              Instance = httpContext.Request.Path 
          }

        
        });
        }
       
       
    }
}