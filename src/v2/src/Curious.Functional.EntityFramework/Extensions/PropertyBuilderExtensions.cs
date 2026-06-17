using Curious.Functional.EntityFramework.Converters;
using Curious.Functional.Monads;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Curious.Functional.EntityFramework.Extensions;

public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Configures the property to store <see cref="Option{T}"/> as a nullable reference column.
    /// <c>None</c> maps to <c>NULL</c>; <c>Some(value)</c> maps to the value directly.
    /// For struct-typed <typeparamref name="T"/>, use <see cref="HasNullableOptionConversion{T}"/> instead.
    /// </summary>
    public static PropertyBuilder<Option<T>> HasOptionConversion<T>(
        this PropertyBuilder<Option<T>> builder)
        where T : class =>
        builder.HasConversion(new OptionConverter<T>());

    /// <summary>
    /// Configures the property to store <see cref="Option{T}"/> as a nullable value-type column.
    /// <c>None</c> maps to <c>NULL</c>; <c>Some(value)</c> maps to the value directly.
    /// For class-typed <typeparamref name="T"/>, use <see cref="HasOptionConversion{T}"/> instead.
    /// </summary>
    public static PropertyBuilder<Option<T>> HasNullableOptionConversion<T>(
        this PropertyBuilder<Option<T>> builder)
        where T : struct =>
        builder.HasConversion(new OptionNullableConverter<T>());
}
