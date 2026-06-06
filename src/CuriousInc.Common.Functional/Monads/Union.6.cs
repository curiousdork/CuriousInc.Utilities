namespace CuriousInc.Common.Functional.Monads;
/// <summary>
/// Represents discriminated union with six possible value types.
/// Use <see cref="Union{T1, T2, T3, T4, T5, T6}"/> when API must model one of six typed domain outcomes and no branch should imply error semantics by convention.
/// </summary>
/// <typeparam name="T1">First possible value type.</typeparam>
/// <typeparam name="T2">Second possible value type.</typeparam>
/// <typeparam name="T3">Third possible value type.</typeparam>
/// <typeparam name="T4">Fourth possible value type.</typeparam>
/// <typeparam name="T5">Fifth possible value type.</typeparam>
/// <typeparam name="T6">Sixth possible value type.</typeparam>
public readonly record struct Union<T1, T2, T3, T4, T5, T6>
{ // Begin Union class
    private enum Variant : byte
    {
        None = 0,
        T1 = 1,
        T2 = 2,
        T3 = 3,
        T4 = 4,
        T5 = 5,
        T6 = 6,
    }

    private readonly Variant _tag = Variant.None;
    private readonly T1? _t1 = default;
    private readonly T2? _t2 = default;
    private readonly T3? _t3 = default;
    private readonly T4? _t4 = default;
    private readonly T5? _t5 = default;
    private readonly T6? _t6 = default;
    /// <summary>Gets first value without checking active variant.</summary>
    public readonly T1 T1Value => _t1!;
    /// <summary>Gets second value without checking active variant.</summary>
    public readonly T2 T2Value => _t2!;
    /// <summary>Gets third value without checking active variant.</summary>
    public readonly T3 T3Value => _t3!;
    /// <summary>Gets fourth value without checking active variant.</summary>
    public readonly T4 T4Value => _t4!;
    /// <summary>Gets fifth value without checking active variant.</summary>
    public readonly T5 T5Value => _t5!;
    /// <summary>Gets sixth value without checking active variant.</summary>
    public readonly T6 T6Value => _t6!;

    private Union(T1 value) => (_tag, _t1) = (Variant.T1, value);
    private Union(T2 value) => (_tag, _t2) = (Variant.T2, value);
    private Union(T3 value) => (_tag, _t3) = (Variant.T3, value);
    private Union(T4 value) => (_tag, _t4) = (Variant.T4, value);
    private Union(T5 value) => (_tag, _t5) = (Variant.T5, value);
    private Union(T6 value) => (_tag, _t6) = (Variant.T6, value);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T1 value) => new(value);
    public static implicit operator T1(Union<T1, T2, T3, T4, T5, T6> value) => value.T1Value;
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T2 value) => new(value);
    public static implicit operator T2(Union<T1, T2, T3, T4, T5, T6> value) => value.T2Value;
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T3 value) => new(value);
    public static implicit operator T3(Union<T1, T2, T3, T4, T5, T6> value) => value.T3Value;
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T4 value) => new(value);
    public static implicit operator T4(Union<T1, T2, T3, T4, T5, T6> value) => value.T4Value;
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T5 value) => new(value);
    public static implicit operator T5(Union<T1, T2, T3, T4, T5, T6> value) => value.T5Value;
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T6 value) => new(value);
    public static implicit operator T6(Union<T1, T2, T3, T4, T5, T6> value) => value.T6Value;
    /// <summary>Attempts to read first variant.</summary>
    public bool TryGetValue(out T1? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T1 => (true, _t1),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>Attempts to read second variant.</summary>
    public bool TryGetValue(out T2? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T2 => (true, _t2),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>Attempts to read third variant.</summary>
    public bool TryGetValue(out T3? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T3 => (true, _t3),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>Attempts to read fourth variant.</summary>
    public bool TryGetValue(out T4? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T4 => (true, _t4),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>Attempts to read fifth variant.</summary>
    public bool TryGetValue(out T5? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T5 => (true, _t5),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>Attempts to read sixth variant.</summary>
    public bool TryGetValue(out T6? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T6 => (true, _t6),
            _ => (false, default)};
        value = temp;
        return result;
    }

    /// <summary>Returns first variant or throws when another variant is active.</summary>
    public T1 UnwrapT1() => _tag switch
    {
        Variant.T1 => _t1!,
        _ => throw new IndexOutOfRangeException("T1 is null.")};
    /// <summary>Returns first variant or fallback when another variant is active.</summary>
    public T1 UnwrapT1OrElse(T1 defaultValue) => _tag switch
    {
        Variant.T1 => _t1!,
        _ => defaultValue
    };
    /// <summary>Returns second variant or throws when another variant is active.</summary>
    public T2 UnwrapT2() => _tag switch
    {
        Variant.T2 => _t2!,
        _ => throw new IndexOutOfRangeException("T2 is null.")};
    /// <summary>Returns second variant or fallback when another variant is active.</summary>
    public T2 UnwrapT2OrElse(T2 defaultValue) => _tag switch
    {
        Variant.T2 => _t2!,
        _ => defaultValue
    };
    /// <summary>Returns third variant or throws when another variant is active.</summary>
    public T3 UnwrapT3() => _tag switch
    {
        Variant.T3 => _t3!,
        _ => throw new IndexOutOfRangeException("T3 is null.")};
    /// <summary>Returns third variant or fallback when another variant is active.</summary>
    public T3 UnwrapT3OrElse(T3 defaultValue) => _tag switch
    {
        Variant.T3 => _t3!,
        _ => defaultValue
    };
    /// <summary>Returns fourth variant or throws when another variant is active.</summary>
    public T4 UnwrapT4() => _tag switch
    {
        Variant.T4 => _t4!,
        _ => throw new IndexOutOfRangeException("T4 is null.")};
    /// <summary>Returns fourth variant or fallback when another variant is active.</summary>
    public T4 UnwrapT4OrElse(T4 defaultValue) => _tag switch
    {
        Variant.T4 => _t4!,
        _ => defaultValue
    };
    /// <summary>Returns fifth variant or throws when another variant is active.</summary>
    public T5 UnwrapT5() => _tag switch
    {
        Variant.T5 => _t5!,
        _ => throw new IndexOutOfRangeException("T5 is null.")};
    /// <summary>Returns fifth variant or fallback when another variant is active.</summary>
    public T5 UnwrapT5OrElse(T5 defaultValue) => _tag switch
    {
        Variant.T5 => _t5!,
        _ => defaultValue
    };
    /// <summary>Returns sixth variant or throws when another variant is active.</summary>
    public T6 UnwrapT6() => _tag switch
    {
        Variant.T6 => _t6!,
        _ => throw new IndexOutOfRangeException("T6 is null.")};
    /// <summary>Returns sixth variant or fallback when another variant is active.</summary>
    public T6 UnwrapT6OrElse(T6 defaultValue) => _tag switch
    {
        Variant.T6 => _t6!,
        _ => defaultValue
    };
    /// <summary>Pattern matches active variant across all six cases.</summary>
    public TResult Match<TResult>(Func<T1, TResult> func1, Func<T2, TResult> func2, Func<T3, TResult> func3, Func<T4, TResult> func4, Func<T5, TResult> func5, Func<T6, TResult> func6) => _tag switch
    {
        Variant.T1 => func1(_t1!),
        Variant.T2 => func2(_t2!),
        Variant.T3 => func3(_t3!),
        Variant.T4 => func4(_t4!),
        Variant.T5 => func5(_t5!),
        Variant.T6 => func6(_t6!),
        _ => throw new ArgumentOutOfRangeException($"{_tag} could not be found.")};
    /// <summary>Transforms first variant into new union when first variant is active.</summary>
    public Union<T1, T2, T3, T4, T5, T6> Apply(Func<T1, Union<T1, T2, T3, T4, T5, T6>> functor) => _tag switch
    {
        Variant.T1 => functor(_t1!),
        _ => throw new ArgumentNullException()};
    /// <summary>Transforms second variant into new union when second variant is active.</summary>
    public Union<T1, T2, T3, T4, T5, T6> Apply(Func<T2, Union<T1, T2, T3, T4, T5, T6>> functor) => _tag switch
    {
        Variant.T2 => functor(_t2!),
        _ => throw new ArgumentNullException()};
    /// <summary>Transforms third variant into new union when third variant is active.</summary>
    public Union<T1, T2, T3, T4, T5, T6> Apply(Func<T3, Union<T1, T2, T3, T4, T5, T6>> functor) => _tag switch
    {
        Variant.T3 => functor(_t3!),
        _ => throw new ArgumentNullException()};
    /// <summary>Transforms fourth variant into new union when fourth variant is active.</summary>
    public Union<T1, T2, T3, T4, T5, T6> Apply(Func<T4, Union<T1, T2, T3, T4, T5, T6>> functor) => _tag switch
    {
        Variant.T4 => functor(_t4!),
        _ => throw new ArgumentNullException()};
    /// <summary>Transforms fifth variant into new union when fifth variant is active.</summary>
    public Union<T1, T2, T3, T4, T5, T6> Apply(Func<T5, Union<T1, T2, T3, T4, T5, T6>> functor) => _tag switch
    {
        Variant.T5 => functor(_t5!),
        _ => throw new ArgumentNullException()};
    /// <summary>Transforms sixth variant into new union when sixth variant is active.</summary>
    public Union<T1, T2, T3, T4, T5, T6> Apply(Func<T6, Union<T1, T2, T3, T4, T5, T6>> functor) => _tag switch
    {
        Variant.T6 => functor(_t6!),
        _ => throw new ArgumentNullException()};
} // End of Union Class
