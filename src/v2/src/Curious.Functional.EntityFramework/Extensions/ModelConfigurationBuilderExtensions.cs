using Curious.Functional.EntityFramework.Converters;
using Curious.Functional.Monads;
using Microsoft.EntityFrameworkCore;

namespace Curious.Functional.EntityFramework.Extensions;

public static class ModelConfigurationBuilderExtensions
{
    /// <summary>
    /// Globally registers <see cref="OptionConverter{T}"/> for all <see cref="Option{T}"/> properties
    /// where <typeparamref name="T"/> is a reference type. Call once per T from <c>ConfigureConventions</c>.
    /// </summary>
    public static ModelConfigurationBuilder UseOptionConversion<T>(
        this ModelConfigurationBuilder builder)
        where T : class
    {
        builder.Properties<Option<T>>().HaveConversion<OptionConverter<T>>();
        return builder;
    }

    /// <summary>
    /// Globally registers <see cref="OptionNullableConverter{T}"/> for all <see cref="Option{T}"/> properties
    /// where <typeparamref name="T"/> is a value type. Call once per T from <c>ConfigureConventions</c>.
    /// </summary>
    public static ModelConfigurationBuilder UseNullableOptionConversion<T>(
        this ModelConfigurationBuilder builder)
        where T : struct
    {
        builder.Properties<Option<T>>().HaveConversion<OptionNullableConverter<T>>();
        return builder;
    }

    /// <summary>
    /// Globally registers <see cref="EitherConverter{L,R}"/> (with default JSON options) for all
    /// <see cref="Either{L,R}"/> properties of the given type arguments. Call once per L/R combination
    /// from <c>ConfigureConventions</c>. For custom <c>JsonSerializerOptions</c>, configure via
    /// <see cref="PropertyBuilderExtensions.HasEitherConversion{L,R}"/> per property instead.
    /// </summary>
    public static ModelConfigurationBuilder UseEitherConversion<L, R>(
        this ModelConfigurationBuilder builder)
        where L : notnull
        where R : notnull
    {
        builder.Properties<Either<L, R>>().HaveConversion<EitherConverter<L, R>>();
        return builder;
    }

    /// <summary>
    /// Globally registers <see cref="ResultConverter{T}"/> (with default JSON options) for all
    /// <see cref="Result{T}"/> properties of the given type argument. Call once per T from
    /// <c>ConfigureConventions</c>. For custom <c>JsonSerializerOptions</c> (required when Error
    /// subtypes need polymorphic deserialization), configure via
    /// <see cref="PropertyBuilderExtensions.HasResultConversion{T}"/> per property instead.
    /// </summary>
    public static ModelConfigurationBuilder UseResultConversion<T>(
        this ModelConfigurationBuilder builder)
        where T : notnull
    {
        builder.Properties<Result<T>>().HaveConversion<ResultConverter<T>>();
        return builder;
    }
}
