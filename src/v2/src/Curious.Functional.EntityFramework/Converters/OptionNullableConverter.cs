using Curious.Functional.Monads;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Curious.Functional.EntityFramework.Converters;

/// <summary>
/// Converts <see cref="Option{T}"/> to a nullable value type (<see cref="Nullable{T}"/>) for persistence.
/// Maps <c>Some(value)</c> → column value and <c>None</c> → <c>NULL</c>.
/// For class-typed <typeparamref name="T"/>, use <see cref="OptionConverter{T}"/> instead.
/// </summary>
public class OptionNullableConverter<T> : ValueConverter<Option<T>, T?>
    where T : struct
{
    public OptionNullableConverter() : base(
        option => option.HasValue ? option.Unwrap() : null,
        value => value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None)
    { }
}
