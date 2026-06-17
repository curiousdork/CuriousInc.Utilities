using Curious.Functional.AspNet.ExceptionHandling;
using Curious.Functional.AspNet.Http;
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
    /// Registers the <see cref="GlobalExceptionHandler"/> which catches every unhandled exception and
    /// maps it to a <see cref="ProblemDetails"/> response. Three cases are handled in priority order:
    /// <list type="number">
    ///   <item><see cref="Exceptions.HttpErrorException"/> — typed HTTP errors thrown directly.</item>
    ///   <item>Any exception whose <c>Data["HttpError"]</c> is an <see cref="HttpError"/>.</item>
    ///   <item>All other exceptions — mapped to 500 Internal Server Error and logged at <c>Error</c> level.</item>
    /// </list>
    /// <para>
    /// Requires <c>app.UseExceptionHandler()</c> in the middleware pipeline.
    /// </para>
    /// </summary>
    public static IServiceCollection AddCuriousExceptionHandler(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}
