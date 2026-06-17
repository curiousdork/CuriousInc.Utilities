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
}
