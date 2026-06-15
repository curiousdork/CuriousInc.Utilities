using System.Text.Json;
using System.Text.Json.Serialization;
using Curious.Functional.Monads;
using Curious.Functional.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Curious.Functional.EntityFramework.Converters;

/// <summary>
/// Converts <see cref="Result{T}"/> to a JSON string column.
/// <para>
/// Because <see cref="Error"/> is abstract, the <paramref name="options"/> passed to the constructor
/// must configure polymorphic JSON handling for every concrete <see cref="Error"/> subtype used at runtime.
/// See <see href="https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism"/>.
/// </para>
/// </summary>
public class ResultConverter<T> : ValueConverter<Result<T>, string>
    where T : notnull
{
    public ResultConverter(JsonSerializerOptions? options = null) : base(
        result => Serialize(result, options),
        json => Deserialize(json, options))
    { }

    private static string Serialize(Result<T> result, JsonSerializerOptions? options) =>
        JsonSerializer.Serialize(
            result.Match(
                ok:   v => new ResultStorageModel<T> { IsOk = true, Value = v },
                fail: e => new ResultStorageModel<T> { IsOk = false, Error = e }),
            options);

    private static Result<T> Deserialize(string json, JsonSerializerOptions? options)
    {
        var model = JsonSerializer.Deserialize<ResultStorageModel<T>>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize Result value from JSON.");

        return model.IsOk
            ? Result<T>.Ok(model.Value!)
            : Result<T>.Fail(model.Error!);
    }
}

internal sealed class ResultStorageModel<T>
    where T : notnull
{
    [JsonPropertyName("isOk")]
    public bool IsOk { get; set; }

    [JsonPropertyName("value")]
    public T? Value { get; set; }

    [JsonPropertyName("error")]
    public Error? Error { get; set; }
}
