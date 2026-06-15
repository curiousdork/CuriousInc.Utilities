using Curious.Functional.Monads;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Curious.Functional.EntityFramework.Converters;

/// <summary>
/// Converts <see cref="Option{T}"/> to a nullable reference type for persistence.
/// Maps <c>Some(value)</c> → column value and <c>None</c> → <c>NULL</c>.
/// For struct-typed <typeparamref name="T"/>, use <see cref="OptionNullableConverter{T}"/> instead.
/// </summary>
public class OptionConverter<T>() : ValueConverter<Option<T>, T?>(option => option.HasValue ? option.Unwrap() : null,
    value => value != null ? Option<T>.Some(value) : Option<T>.None)
    where T : class;
