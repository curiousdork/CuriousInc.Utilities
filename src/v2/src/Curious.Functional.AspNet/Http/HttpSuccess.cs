using System.Text.Json.Serialization;
using Curious.Functional.ValueObjects;

namespace Curious.Functional.AspNet.Http;

/// <summary>
/// Base type for all HTTP success responses.
/// Derived types are generic; register closed forms (e.g. <c>Ok&lt;MyDto&gt;</c>)
/// with <c>[JsonSerializable]</c> in your application's <see cref="JsonSerializerContext"/>.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
public abstract record HttpSuccess(int StatusCode) : Status;

public sealed record Ok<T>(T Value) : HttpSuccess(200);
public sealed record Created<T>(T Value) : HttpSuccess(201);
public sealed record Accepted<T>(T Value) : HttpSuccess(202);
public sealed record NoContent<TId>(TId Id) : HttpSuccess(204);
public sealed record PartialContent<T>(T Value) : HttpSuccess(206);
