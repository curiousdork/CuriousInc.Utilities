namespace CuriousInc.Common.Functional.Monads;
/// <summary>
/// Represents discriminated union with two possible value types.
/// Use <see cref="Union{T1, T2}"/> when value can be one of multiple domain shapes and neither branch is inherently "success" or "failure".
/// Prefer <see cref="Either{L,R}"/> or <see cref="Result{T}"/> when branch meaning should be success/failure by convention.
/// </summary>
/// <typeparam name="T1">First possible value type.</typeparam>
/// <typeparam name="T2">Second possible value type.</typeparam>
public readonly record struct Union<T1, T2>
{ // Begin Union class
    private enum Variant : byte
    {
        None = 0,
        T1 = 1,
        T2 = 2,
    }

    private readonly Variant _tag = Variant.None;
    private readonly T1? _t1 = default;
    private readonly T2? _t2 = default;
    /// <summary>
    /// Gets first value without checking active variant.
    /// Use only when caller already knows instance contains <typeparamref name="T1"/>.
    /// </summary>
    public readonly T1 T1Value => _t1!;
    /// <summary>
    /// Gets second value without checking active variant.
    /// Use only when caller already knows instance contains <typeparamref name="T2"/>.
    /// </summary>
    public readonly T2 T2Value => _t2!;

    private Union(T1 value) => (_tag, _t1) = (Variant.T1, value);
    private Union(T2 value) => (_tag, _t2) = (Variant.T2, value);
    public static implicit operator Union<T1, T2>(T1 value) => new(value);
    public static implicit operator T1(Union<T1, T2> value) => value.T1Value;
    public static implicit operator Union<T1, T2>(T2 value) => new(value);
    public static implicit operator T2(Union<T1, T2> value) => value.T2Value;
    /// <summary>
    /// Attempts to read first variant.
    /// Use for non-throwing extraction when union may hold another type.
    /// </summary>
    /// <param name="value">Receives first value when active; otherwise default value of <typeparamref name="T1"/>.</param>
    /// <returns><see langword="true"/> when first variant is active; otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(out T1? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T1 => (true, _t1),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>
    /// Attempts to read second variant.
    /// Use for non-throwing extraction when union may hold another type.
    /// </summary>
    /// <param name="value">Receives second value when active; otherwise default value of <typeparamref name="T2"/>.</param>
    /// <returns><see langword="true"/> when second variant is active; otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(out T2? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T2 => (true, _t2),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>
    /// Returns first variant.
    /// Use only when caller expects first variant and wants invariant violation to throw.
    /// </summary>
    /// <returns>First variant value.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when active variant is not <typeparamref name="T1"/>.</exception>
    public T1 UnwrapT1() => _tag switch
    {
        Variant.T1 => _t1!,
        _ => throw new IndexOutOfRangeException("T1 is null.")};
    /// <summary>
    /// Returns first variant or supplied fallback.
    /// Use when first variant is preferred but not required.
    /// </summary>
    /// <param name="defaultValue">Fallback value returned when union does not contain first variant.</param>
    /// <returns>First variant or <paramref name="defaultValue"/>.</returns>
    public T1 UnwrapT1OrElse(T1 defaultValue) => _tag switch
    {
        Variant.T1 => _t1!,
        _ => defaultValue
    };
    /// <summary>
    /// Returns second variant.
    /// Use only when caller expects second variant and wants invariant violation to throw.
    /// </summary>
    /// <returns>Second variant value.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when active variant is not <typeparamref name="T2"/>.</exception>
    public T2 UnwrapT2() => _tag switch
    {
        Variant.T2 => _t2!,
        _ => throw new IndexOutOfRangeException("T2 is null.")};
    /// <summary>
    /// Returns second variant or supplied fallback.
    /// Use when second variant is preferred but not required.
    /// </summary>
    /// <param name="defaultValue">Fallback value returned when union does not contain second variant.</param>
    /// <returns>Second variant or <paramref name="defaultValue"/>.</returns>
    public T2 UnwrapT2OrElse(T2 defaultValue) => _tag switch
    {
        Variant.T2 => _t2!,
        _ => defaultValue
    };
    /// <summary>
    /// Pattern matches active variant.
    /// Use when all variants must be handled explicitly.
    /// </summary>
    /// <typeparam name="TResult">Common return type for both branches.</typeparam>
    /// <param name="func1">Handler for first variant.</param>
    /// <param name="func2">Handler for second variant.</param>
    /// <returns>Value produced by active branch handler.</returns>
    public TResult Match<TResult>(Func<T1, TResult> func1, Func<T2, TResult> func2) => _tag switch
    {
        Variant.T1 => func1(_t1!),
        Variant.T2 => func2(_t2!),
        _ => throw new ArgumentOutOfRangeException($"{_tag} could not be found.")};
    /// <summary>
    /// Replaces first variant with new union value produced by <paramref name="functor"/>.
    /// Use when only first branch should continue through transformation.
    /// </summary>
    /// <param name="functor">Transformation executed when first variant is active.</param>
    /// <returns>Transformed union.</returns>
    public Union<T1, T2> Apply(Func<T1, Union<T1, T2>> functor) => _tag switch
    {
        Variant.T1 => functor(_t1!),
        _ => throw new ArgumentNullException()};
    /// <summary>
    /// Replaces second variant with new union value produced by <paramref name="functor"/>.
    /// Use when only second branch should continue through transformation.
    /// </summary>
    /// <param name="functor">Transformation executed when second variant is active.</param>
    /// <returns>Transformed union.</returns>
    public Union<T1, T2> Apply(Func<T2, Union<T1, T2>> functor) => _tag switch
    {
        Variant.T2 => functor(_t2!),
        _ => throw new ArgumentNullException()};
} // End of Union Class
