using System.Text.Json.Serialization;
using Curious.Functional.AspNet.Http;

namespace Curious.Functional.AspNet.Serialization;

/// <summary>
/// AOT-safe <see cref="JsonSerializerContext"/> for all non-generic HTTP status code types.
/// </summary>
/// <remarks>
/// Success types (<see cref="Ok{T}"/>, <see cref="Created{T}"/>, etc.) are generic and cannot
/// be registered here with open type parameters. Add a <c>[JsonSerializable(typeof(Ok&lt;MyDto&gt;))]</c>
/// attribute on your own <see cref="JsonSerializerContext"/> and chain the contexts via
/// <c>JsonSerializerOptions.TypeInfoResolverChain</c>.
/// </remarks>
[JsonSerializable(typeof(HttpError))]
[JsonSerializable(typeof(BadRequestError))]
[JsonSerializable(typeof(UnauthorizedError))]
[JsonSerializable(typeof(ForbiddenError))]
[JsonSerializable(typeof(NotFoundError))]
[JsonSerializable(typeof(MethodNotAllowedError))]
[JsonSerializable(typeof(RequestTimeoutError))]
[JsonSerializable(typeof(ConflictError))]
[JsonSerializable(typeof(GoneError))]
[JsonSerializable(typeof(UnsupportedMediaTypeError))]
[JsonSerializable(typeof(UnprocessableEntityError))]
[JsonSerializable(typeof(TooManyRequestsError))]
[JsonSerializable(typeof(InternalServerError))]
[JsonSerializable(typeof(NotImplementedError))]
[JsonSerializable(typeof(BadGatewayError))]
[JsonSerializable(typeof(ServiceUnavailableError))]
[JsonSerializable(typeof(GatewayTimeoutError))]
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class HttpStatusCodeJsonContext : JsonSerializerContext;
