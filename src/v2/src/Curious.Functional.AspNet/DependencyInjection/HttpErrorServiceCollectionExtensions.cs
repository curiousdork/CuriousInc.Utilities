using Curious.Functional.AspNet.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Curious.Functional.AspNet.DependencyInjection;

public static class HttpErrorServiceCollectionExtensions
{
    /// <summary>
    /// Registers ASP.NET Core problem details services and maps <see cref="HttpError"/> instances
    /// stored in <c>HttpContext.Items["Error"]</c> into the outgoing <see cref="ProblemDetails"/> response.
    /// </summary>
    /// <remarks>
    /// Store the error before writing the response:
    /// <code>
    /// httpContext.Items["Error"] = myHttpError;
    /// </code>
    /// Then call <c>app.UseStatusCodePages()</c> or rely on the problem details middleware
    /// to produce the final response. For minimal APIs, prefer calling
    /// <see cref="HttpErrorExtensions.ToResult"/> directly instead.
    /// </remarks>
    public static IServiceCollection AddCuriousProblemDetails(
        this IServiceCollection services,
        Action<ProblemDetailsOptions>? configure = null)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                if (ctx.HttpContext.Items.TryGetValue("Error", out var raw) && raw is HttpError error)
                {
                    var pd = error.ToProblemDetails();
                    ctx.ProblemDetails.Type   = pd.Type;
                    ctx.ProblemDetails.Title  = pd.Title;
                    ctx.ProblemDetails.Status = pd.Status;
                    ctx.ProblemDetails.Detail = pd.Detail;
                }
            };

            configure?.Invoke(options);
        });

        return services;
    }

    /// <summary>
    /// Registers an <see cref="IExceptionHandler"/> that maps any exception whose
    /// <see cref="Exception.Data"/> contains an <see cref="HttpError"/> under the key <c>"HttpError"</c>
    /// to a <see cref="ProblemDetails"/> response.
    /// </summary>
    public static IServiceCollection AddCuriousExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<HttpErrorExceptionHandler>();
        return services;
    }
}

file sealed class HttpErrorExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception.Data["HttpError"] is not HttpError error)
            return false;

        var pd = error.ToProblemDetails();

        httpContext.Response.StatusCode  = pd.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);
        return true;
    }
}
