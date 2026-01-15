namespace CuriousInc.Common.Functional.Monads;
public readonly record struct Union<T1, T2, T3>
{ // Begin Union class
    private enum Variant : byte
    {
        None = 0,
        T1 = 1,
        T2 = 2,
        T3 = 3,
    }

    private readonly Variant _tag = Variant.None;
    private readonly T1? _t1 = default;
    private readonly T2? _t2 = default;
    private readonly T3? _t3 = default;
    public readonly T1 T1Value => _t1!;
    public readonly T2 T2Value => _t2!;
    public readonly T3 T3Value => _t3!;

    private Union(T1 value) => (_tag, _t1) = (Variant.T1, value);
    private Union(T2 value) => (_tag, _t2) = (Variant.T2, value);
    private Union(T3 value) => (_tag, _t3) = (Variant.T3, value);
    public static implicit operator Union<T1, T2, T3>(T1 value) => new(value);
    public static implicit operator T1(Union<T1, T2, T3> value) => value.T1Value;
    public static implicit operator Union<T1, T2, T3>(T2 value) => new(value);
    public static implicit operator T2(Union<T1, T2, T3> value) => value.T2Value;
    public static implicit operator Union<T1, T2, T3>(T3 value) => new(value);
    public static implicit operator T3(Union<T1, T2, T3> value) => value.T3Value;
    public bool TryGetValue(out T1? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T1 => (true, _t1),
            _ => (false, default)};
        value = temp;
        return result;
    }

    public bool TryGetValue(out T2? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T2 => (true, _t2),
            _ => (false, default)};
        value = temp;
        return result;
    }

    public bool TryGetValue(out T3? value)
    {
        var(result, temp) = _tag switch
        {
            Variant.T3 => (true, _t3),
            _ => (false, default)};
        value = temp;
        return result;
    }

    public T1 UnwrapT1() => _tag switch
    {
        Variant.T1 => _t1!,
        _ => throw new IndexOutOfRangeException("T1 is null.")};
    public T1 UnwrapT1OrElse(T1 defaultValue) => _tag switch
    {
        Variant.T1 => _t1!,
        _ => defaultValue
    };
    public T2 UnwrapT2() => _tag switch
    {
        Variant.T2 => _t2!,
        _ => throw new IndexOutOfRangeException("T2 is null.")};
    public T2 UnwrapT2OrElse(T2 defaultValue) => _tag switch
    {
        Variant.T2 => _t2!,
        _ => defaultValue
    };
    public T3 UnwrapT3() => _tag switch
    {
        Variant.T3 => _t3!,
        _ => throw new IndexOutOfRangeException("T3 is null.")};
    public T3 UnwrapT3OrElse(T3 defaultValue) => _tag switch
    {
        Variant.T3 => _t3!,
        _ => defaultValue
    };
    public TResult Match<TResult>(Func<T1, TResult> func1, Func<T2, TResult> func2, Func<T3, TResult> func3) => _tag switch
    {
        Variant.T1 => func1(_t1!),
        Variant.T2 => func2(_t2!),
        Variant.T3 => func3(_t3!),
        _ => throw new ArgumentOutOfRangeException($"{_tag} could not be found.")};
    public Union<T1, T2, T3> Apply(Func<T1, Union<T1, T2, T3>> functor) => _tag switch
    {
        Variant.T1 => functor(_t1!),
        _ => throw new ArgumentNullException()};
    public Union<T1, T2, T3> Apply(Func<T2, Union<T1, T2, T3>> functor) => _tag switch
    {
        Variant.T2 => functor(_t2!),
        _ => throw new ArgumentNullException()};
    public Union<T1, T2, T3> Apply(Func<T3, Union<T1, T2, T3>> functor) => _tag switch
    {
        Variant.T3 => functor(_t3!),
        _ => throw new ArgumentNullException()};
} // End of Union Class
