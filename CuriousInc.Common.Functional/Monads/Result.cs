namespace CuriousInc.Common.Functional.Monads;

public readonly record struct Result<T>
{
    private readonly Error? _error = default;
    private readonly T? _value = default;

    private Result(Error error) => _error = error;
    private Result(T value) => _value = value;


    public static Result<T> Ok(T? value) => value switch
    {
        null => new Result<T>(Error.New($"{nameof(value)} is null", ErrorType.IsNull)),
        _ => new Result<T>(value)
    };
    public static Result<T> Fail(Error error) => new(error);
    
    public static implicit operator Result<T>(T? value) => Ok(value);
    public static implicit operator T(Result<T> value) => value.TryGetValue(out var result) ? result! : throw new InvalidOperationException("Cannot unwrap a failed Result.");

    public bool IsRight => _value is not null;
    public bool IsLeft => _value is not null;

    public TResult Match<TResult>(Func<T, TResult> ok, Func<Error, TResult> fail)
    {
        if (IsLeft) return fail(_error!);
        return ok(_value!);
    }

    public bool TryGetValue(out T? value)
    {
        if (IsLeft)
        {
            value = default;
            return false;
        }

        value = _value;
        return true;
    }
    
    public override string ToString() => Match(v => v.ToString(), e => e.ToString());
}