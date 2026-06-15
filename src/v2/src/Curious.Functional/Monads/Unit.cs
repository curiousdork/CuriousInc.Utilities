namespace Curious.Functional.Monads;

/// <summary>
/// Represents a type with a single value. This is typically used in functional programming 
/// to represent the absence of a meaningful value, similar to <see langword="void"/>, 
/// but as a first-class type that can be used as a generic argument.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Unit"/> is useful when a generic signature requires a return type, but no data 
/// needs to be returned.
/// </para>
/// <example>
/// Usage with a hypothetical <c>Result</c> type:
/// <code>
/// public Result&lt;Unit&gt; SaveChanges()
/// {
///     // Perform action...
///     return Unit.Default;
/// }
/// </code>
/// </example>
/// </remarks>
public readonly record struct Unit
{
    /// <summary>
    /// Gets the single, default instance of the <see cref="Unit"/> type.
    /// </summary>
    public static Unit Default => new();
}