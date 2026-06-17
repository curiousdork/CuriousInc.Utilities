using Curious.Functional.AspNet.Exceptions;
using Curious.Functional.AspNet.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Curious.Functional.AspNet.ExceptionHandling;

internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = ResolveError(exception);

        if (error.StatusCode >= 500)
            logger.LogError(exception, "Unhandled exception mapped to {StatusCode}: {Message}", error.StatusCode, exception.Message);
        else
            logger.LogDebug(exception, "HTTP {StatusCode} error: {Message}", error.StatusCode, error.Message);

        var pd = error.ToProblemDetails();

        httpContext.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext    = httpContext,
            Exception      = exception,
            ProblemDetails = pd
        });

        return true;
    }

    private static HttpError ResolveError(Exception exception) => exception switch
    {
        HttpErrorException { Error: var e }             => e,
        _ when exception.Data["HttpError"] is HttpError e => e,
        _                                                  => new InternalServerError()
    };
}
