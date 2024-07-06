using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.API.Middlewares
{
    public class EnvironmentControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

        public EnvironmentControlMiddleware(RequestDelegate next, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (_environment.IsDevelopment())
            {
                await _next(context);
                return;
            }
            
            if (context.Request.Path.HasValue)
            {
                var containsDev = context.Request.Path.Value.Contains("/dev/");

                if (containsDev)
                {
                    var problemService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
                    var problemDetailsContext = new ProblemDetailsContext
                    {
                        HttpContext = context,
                        AdditionalMetadata = new EndpointMetadataCollection("get endpoint metadata"),
                        ProblemDetails = new ProblemDetails
                        {
                            Instance = $"{_environment.EnvironmentName}-{_environment.ApplicationName}",
                        }
                    };
                    problemDetailsContext.ProblemDetails.Status = (int)HttpStatusCode.MethodNotAllowed;
                    problemDetailsContext.ProblemDetails.Title = "MethodNotAllowed";
                    problemDetailsContext.ProblemDetails.Detail = $"{context.Request.Path.Value} is forbidden for non-development environments.";
                    problemDetailsContext.ProblemDetails.Type = nameof(MethodAccessException);
                    context.Response.StatusCode = 405;

                    await problemService.WriteAsync(problemDetailsContext);
                    return;
                }
            }

            await _next(context);
        }
    }
}
