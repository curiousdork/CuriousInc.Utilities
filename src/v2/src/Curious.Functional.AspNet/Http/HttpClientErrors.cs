namespace Curious.Functional.AspNet.Http;

/// <summary>400 — The request was malformed or contained invalid parameters.</summary>
public sealed record BadRequestError(string Message = "Bad Request") : HttpError(400, Message);

/// <summary>401 — Authentication is required and has not been provided or is invalid.</summary>
public sealed record UnauthorizedError(string Message = "Unauthorized") : HttpError(401, Message);

/// <summary>403 — The caller is authenticated but does not have permission to access the resource.</summary>
public sealed record ForbiddenError(string Message = "Forbidden") : HttpError(403, Message);

/// <summary>404 — The requested resource could not be found.</summary>
public sealed record NotFoundError(string Message = "Not Found") : HttpError(404, Message);

/// <summary>405 — The HTTP method used is not supported by the target resource.</summary>
public sealed record MethodNotAllowedError(string Message = "Method Not Allowed") : HttpError(405, Message);

/// <summary>408 — The server timed out waiting for the request.</summary>
public sealed record RequestTimeoutError(string Message = "Request Timeout") : HttpError(408, Message);

/// <summary>409 — The request conflicts with the current state of the resource.</summary>
public sealed record ConflictError(string Message = "Conflict") : HttpError(409, Message);

/// <summary>410 — The resource has been permanently removed and will not be available again.</summary>
public sealed record GoneError(string Message = "Gone") : HttpError(410, Message);

/// <summary>415 — The request payload format is not supported by the server.</summary>
public sealed record UnsupportedMediaTypeError(string Message = "Unsupported Media Type") : HttpError(415, Message);

/// <summary>422 — The request was well-formed but contained semantic errors.</summary>
public sealed record UnprocessableEntityError(string Message = "Unprocessable Entity") : HttpError(422, Message);

/// <summary>429 — The caller has exceeded the allowed request rate.</summary>
public sealed record TooManyRequestsError(string Message = "Too Many Requests") : HttpError(429, Message);
