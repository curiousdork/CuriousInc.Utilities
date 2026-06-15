using System.Text.Json;
using System.Text.Json.Serialization;
using Curious.Functional.Monads;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Curious.Functional.EntityFramework.Converters;

/// <summary>
/// Converts <see cref="Either{L,R}"/> to a JSON string column.
/// Pass <see cref="JsonSerializerOptions"/> when <typeparamref name="L"/> or <typeparamref name="R"/>
/// require custom serialization (e.g. polymorphic types or custom converters).
/// </summary>
public class EitherConverter<L, R> : ValueConverter<Either<L, R>, string>
    where L : notnull
    where R : notnull
{
    public EitherConverter(JsonSerializerOptions? options = null) : base(
        either => Serialize(either, options),
        json => Deserialize(json, options))
    { }

    private static string Serialize(Either<L, R> either, JsonSerializerOptions? options) =>
        JsonSerializer.Serialize(
            either.Match(
                right: r => new EitherStorageModel<L, R> { IsRight = true, Right = r },
                left:  l => new EitherStorageModel<L, R> { IsRight = false, Left = l }),
            options);

    private static Either<L, R> Deserialize(string json, JsonSerializerOptions? options)
    {
        var model = JsonSerializer.Deserialize<EitherStorageModel<L, R>>(json, options)
            ?? throw new InvalidOperationException("Failed to deserialize Either value from JSON.");

        return model.IsRight
            ? Either<L, R>.Right(model.Right!)
            : Either<L, R>.Left(model.Left!);
    }
}

internal sealed class EitherStorageModel<L, R>
    where L : notnull
    where R : notnull
{
    [JsonPropertyName("isRight")]
    public bool IsRight { get; set; }

    [JsonPropertyName("right")]
    public R? Right { get; set; }

    [JsonPropertyName("left")]
    public L? Left { get; set; }
}
