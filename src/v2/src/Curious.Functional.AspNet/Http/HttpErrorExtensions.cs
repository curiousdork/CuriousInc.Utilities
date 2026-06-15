using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Curious.Functional.AspNet.Http;

public static class HttpErrorExtensions
{
    private static readonly ConcurrentDictionary<Type, Func<HttpError, ProblemDetails>> Registry = new();

    /// <summary>
    /// Registers a custom <see cref="ProblemDetails"/> factory for <typeparamref name="TError"/>.
    /// The registration is checked before the built-in switch, so derived or generic types
    /// (e.g. <c>NotFoundError&lt;int&gt;</c>) can supply richer mappings.
    /// Call this once at startup, e.g. in <c>Program.cs</c>.
    /// </summary>
    public static void Register<TError>(Func<TError, ProblemDetails> factory) where TError : HttpError =>
        Registry[typeof(TError)] = e => factory((TError)e);

    /// <summary>Removes a previously registered factory for <typeparamref name="TError"/>.</summary>
    public static void Unregister<TError>() where TError : HttpError =>
        Registry.TryRemove(typeof(TError), out _);

    public static ProblemDetails ToProblemDetails(this HttpError error)
    {
        if (Registry.TryGetValue(error.GetType(), out var factory))
            return factory(error);

        return error switch
        {
            BadRequestError e          => e.ToProblemDetails(),
            UnauthorizedError e        => e.ToProblemDetails(),
            ForbiddenError e           => e.ToProblemDetails(),
            NotFoundError e            => e.ToProblemDetails(),
            MethodNotAllowedError e    => e.ToProblemDetails(),
            RequestTimeoutError e      => e.ToProblemDetails(),
            ConflictError e            => e.ToProblemDetails(),
            GoneError e                => e.ToProblemDetails(),
            UnsupportedMediaTypeError e => e.ToProblemDetails(),
            UnprocessableEntityError e => e.ToProblemDetails(),
            TooManyRequestsError e     => e.ToProblemDetails(),
            InternalServerError e      => e.ToProblemDetails(),
            NotImplementedError e      => e.ToProblemDetails(),
            BadGatewayError e          => e.ToProblemDetails(),
            ServiceUnavailableError e  => e.ToProblemDetails(),
            GatewayTimeoutError e      => e.ToProblemDetails(),
            _                          => Build(error.StatusCode, "Error", error.Message)
        };
    }

    public static IResult ToResult(this HttpError error)
    {
        var pd = error.ToProblemDetails();
        return TypedResults.Problem(pd);
    }

    public static ProblemDetails ToProblemDetails(this BadRequestError error) =>
        Build(400, "Bad Request", error.Message);

    public static ProblemDetails ToProblemDetails(this UnauthorizedError error) =>
        Build(401, "Unauthorized", error.Message);

    public static ProblemDetails ToProblemDetails(this ForbiddenError error) =>
        Build(403, "Forbidden", error.Message);

    public static ProblemDetails ToProblemDetails(this NotFoundError error) =>
        Build(404, "Not Found", error.Message);

    public static ProblemDetails ToProblemDetails(this MethodNotAllowedError error) =>
        Build(405, "Method Not Allowed", error.Message);

    public static ProblemDetails ToProblemDetails(this RequestTimeoutError error) =>
        Build(408, "Request Timeout", error.Message);

    public static ProblemDetails ToProblemDetails(this ConflictError error) =>
        Build(409, "Conflict", error.Message);

    public static ProblemDetails ToProblemDetails(this GoneError error) =>
        Build(410, "Gone", error.Message);

    public static ProblemDetails ToProblemDetails(this UnsupportedMediaTypeError error) =>
        Build(415, "Unsupported Media Type", error.Message);

    public static ProblemDetails ToProblemDetails(this UnprocessableEntityError error) =>
        Build(422, "Unprocessable Entity", error.Message);

    public static ProblemDetails ToProblemDetails(this TooManyRequestsError error) =>
        Build(429, "Too Many Requests", error.Message);

    public static ProblemDetails ToProblemDetails(this InternalServerError error) =>
        Build(500, "Internal Server Error", error.Message);

    public static ProblemDetails ToProblemDetails(this NotImplementedError error) =>
        Build(501, "Not Implemented", error.Message);

    public static ProblemDetails ToProblemDetails(this BadGatewayError error) =>
        Build(502, "Bad Gateway", error.Message);

    public static ProblemDetails ToProblemDetails(this ServiceUnavailableError error) =>
        Build(503, "Service Unavailable", error.Message);

    public static ProblemDetails ToProblemDetails(this GatewayTimeoutError error) =>
        Build(504, "Gateway Timeout", error.Message);

    private static ProblemDetails Build(int status, string title, string detail) => new()
    {
        Type   = $"https://httpstatuses.io/{status}",
        Title  = title,
        Status = status,
        Detail = detail
    };
}
