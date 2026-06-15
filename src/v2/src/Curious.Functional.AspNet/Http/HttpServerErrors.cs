namespace Curious.Functional.AspNet.Http;

/// <summary>500 — The server encountered an unexpected condition that prevented it from fulfilling the request.</summary>
public sealed record InternalServerError(string Message = "Internal Server Error") : HttpError(500, Message);

/// <summary>501 — The server does not support the functionality required to fulfill the request.</summary>
public sealed record NotImplementedError(string Message = "Not Implemented") : HttpError(501, Message);

/// <summary>502 — The server, acting as a gateway, received an invalid response from an upstream server.</summary>
public sealed record BadGatewayError(string Message = "Bad Gateway") : HttpError(502, Message);

/// <summary>503 — The server is temporarily unable to handle the request.</summary>
public sealed record ServiceUnavailableError(string Message = "Service Unavailable") : HttpError(503, Message);

/// <summary>504 — The server, acting as a gateway, did not receive a timely response from an upstream server.</summary>
public sealed record GatewayTimeoutError(string Message = "Gateway Timeout") : HttpError(504, Message);
