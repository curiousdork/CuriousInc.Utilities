namespace CuriousInc.Common.Functional.Monads;

public readonly record struct Either<L, R>
{
    private readonly L? _left;
    private readonly R? _value;
    
    private Either(L left) => _left = left;
    private Either(R value) => _value = value;

    // Factory methods for convenience
    public static Either<L, R> Right(R value) => new(value);
    public static Either<L, R> Left(L left) => new(left);

    public static implicit operator Either<L, R>(R value) => Right(value);
    public static implicit operator Either<L, R>(L left) => Left(left);
    
    public bool IsRight => _left is null;
    public bool IsLeft => _left is not null;

    public bool TryGetValue(out R? value)
    {
        if (IsLeft)
        {
            value = default;
            return false;
        }
        
        value = _value;
        return true;
    }

    // Apply - applies an action to the contained value if Right, does nothing if Left
    public void Apply(Action<R> action)
    {
        if (IsRight && _value is not null)
        {
            action(_value);
        }
    }

    // Match - acts like pattern matching for each case
    public TResult Match<TResult>(Func<R, TResult> right, Func<L, TResult> left) =>
        IsRight && _value is not null
            ? right(_value)
            : left(_left!);

    

    // Unwrap - returns value if Right, throws if Left
    public R Unwrap()
    {
        if (IsRight && _value is not null)
            return _value;
        throw new InvalidOperationException($"Tried to unwrap an Left result: {_left}");
    }
    
    public L Unwrapleft() => (IsLeft ? _left : throw new InvalidOperationException("Tried to unwrap a missing left result."))!;

    // UnwrapOrElse - returns value if Right, otherwise calls the fallback function
    public R UnwrapOrElse(Func<L, R> orElse)
    {
        return IsRight && _value is not null ? _value : orElse(_left!);
    }

    // Bind (flatMap) - if Right, applies func to value and flattens result, else propagates Left
    public Either<L,U> Bind<U>(Func<R, Either<L,U>> func)
        where U : notnull
    {
        return IsRight && _value is not null
            ? func(_value)
            : Either<L,U>.Left(_left!);
    }

    // Map - if Right, applies func to valuL,else propagates Left
    public Either<L,U> Map<U>(Func<R, U> func)
        where U : notnull
    {
        return IsRight && _value is not null
            ? Either<L,U>.Right(func(_value))
            : Either<L,U>.Left(_left!);
    }
    
    public override string ToString() => IsRight ? $"Right({_value})" : $"Left({_left})";
}