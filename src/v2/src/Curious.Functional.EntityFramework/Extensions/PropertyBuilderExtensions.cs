using System.Text.Json;
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

    /// <summary>
    /// Configures the property to store <see cref="Either{L,R}"/> as a JSON string column.
    /// Pass <paramref name="options"/> when <typeparamref name="L"/> or <typeparamref name="R"/>
    /// require custom serialization.
    /// </summary>
    public static PropertyBuilder<Either<L, R>> HasEitherConversion<L, R>(
        this PropertyBuilder<Either<L, R>> builder,
        JsonSerializerOptions? options = null)
        where L : notnull
        where R : notnull =>
        builder.HasConversion(new EitherConverter<L, R>(options));

    /// <summary>
    /// Configures the property to store <see cref="Result{T}"/> as a JSON string column.
    /// <paramref name="options"/> must configure polymorphic serialization for all concrete
    /// <see cref="Curious.Functional.ValueObjects.Error"/> subtypes used at runtime.
    /// </summary>
    public static PropertyBuilder<Result<T>> HasResultConversion<T>(
        this PropertyBuilder<Result<T>> builder,
        JsonSerializerOptions? options = null)
        where T : notnull =>
        builder.HasConversion(new ResultConverter<T>(options));
    
}
