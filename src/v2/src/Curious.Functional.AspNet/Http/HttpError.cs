using System.Text.Json.Serialization;
using Curious.Functional.ValueObjects;

namespace Curious.Functional.AspNet.Http;

/// <summary>
/// Base type for all HTTP-specific errors.
/// Carries the <see cref="StatusCode"/> and a human-readable <see cref="Message"/>.
/// Derive from this type to define additional HTTP error variants.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BadRequestError), "BadRequest")]
[JsonDerivedType(typeof(UnauthorizedError), "Unauthorized")]
[JsonDerivedType(typeof(ForbiddenError), "Forbidden")]
[JsonDerivedType(typeof(NotFoundError), "NotFound")]
[JsonDerivedType(typeof(MethodNotAllowedError), "MethodNotAllowed")]
[JsonDerivedType(typeof(RequestTimeoutError), "RequestTimeout")]
[JsonDerivedType(typeof(ConflictError), "Conflict")]
[JsonDerivedType(typeof(GoneError), "Gone")]
[JsonDerivedType(typeof(UnsupportedMediaTypeError), "UnsupportedMediaType")]
[JsonDerivedType(typeof(UnprocessableEntityError), "UnprocessableEntity")]
[JsonDerivedType(typeof(TooManyRequestsError), "TooManyRequests")]
[JsonDerivedType(typeof(InternalServerError), "InternalServerError")]
[JsonDerivedType(typeof(NotImplementedError), "NotImplemented")]
[JsonDerivedType(typeof(BadGatewayError), "BadGateway")]
[JsonDerivedType(typeof(ServiceUnavailableError), "ServiceUnavailable")]
[JsonDerivedType(typeof(GatewayTimeoutError), "GatewayTimeout")]
public abstract record HttpError(int StatusCode, string Message) : Error;
