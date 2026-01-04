namespace CuriousInc.Common.Functional.Monads;

public readonly record struct Option<T>
{
    private readonly T? _value = default;
    private readonly bool _hasValue = false;
    
    private Option(T? value)
    {
        if (value is not null)
        {
            _value = value;
            _hasValue = true;
        }
    }

    public static implicit operator Option<T>(T? value) => value switch
    {
        null => None,
        _ => Some(value)
    };

    public static Option<T> None => new();
    public static Option<T> Some(T? value) => value is not null ? new Option<T>(value) : None;
    
    public bool HasValue => _hasValue;

    public bool TryGetValue(out T? value)
    {
        if (_hasValue)
        {
            value = _value;
            return true;
        }
        
        value = default;
        return false;
    }
    
    // Apply: Applies a function to the value if present, returns a new Option<TResult>
    public Option<TResult> Apply<TResult>(Func<T, TResult> mapper) where TResult : notnull
    {
        return _hasValue ? Option<TResult>.Some(mapper(_value!)) : Option<TResult>.None;
    }

    // Match: Executes some action based on whether the optional has a value or not 
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        return _hasValue ? some(_value!) : none();
    }

    // Unwrap: Gets the value or throws if not present
    public T Unwrap()
    {
        if (!_hasValue)
            throw new InvalidOperationException("No value present in Option.");
        return _value!;
    }

    // UnwrapOrElse: Gets the value or executes a fallback function
    public T UnwrapOrElse(Func<T> fallback)
    {
        return _hasValue ? _value! : fallback();
    }

    public Unit IfNone(Action action)
    {
        if (!_hasValue)
        {
            action();
        }
        
        return Unit.Default;
    }

    public Unit IfSome(Action<T> action)
    {
        if (_hasValue)
        {
            action(_value!);
        }
        
        return Unit.Default;
    }

    public override string ToString()
    {
        return Match(v => $"Some({v})", () => "None");
    }
}