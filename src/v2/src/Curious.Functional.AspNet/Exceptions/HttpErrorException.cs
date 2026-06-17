using Curious.Functional.AspNet.Http;

namespace Curious.Functional.AspNet.Exceptions;

/// <summary>
/// Wraps an <see cref="HttpError"/> as a throwable exception.
/// The global exception handler will extract <see cref="Error"/> and produce the correct
/// <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/> response automatically.
/// </summary>
public sealed class HttpErrorException(HttpError error) : Exception(error.Message)
{
    public HttpError Error { get; } = error;
}
