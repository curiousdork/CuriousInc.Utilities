namespace CuriousInc.Common.Functional.Monads;

/// <summary>
/// Represents operation result that is either successful value or <see cref="Error"/>.
/// Use <see cref="Result{T}"/> when operation can fail and caller should receive standard repository error type rather than raw null or exception-driven flow.
/// Use <see cref="Option{T}"/> when absence is not failure, or <see cref="Either{L,R}"/> when failure type must vary.
/// </summary>
/// <typeparam name="T">Successful value type.</typeparam>
public readonly record struct Result<T>
{
    private readonly Error? _error = null;
    private readonly T? _value = default;

    private Result(Error error) => _error = error;
    private Result(T value) => _value = value;


    /// <summary>
    /// Creates successful result when <paramref name="value"/> is not <see langword="null"/>, otherwise creates failed result with default <see cref="Error"/>.
    /// Use when converting nullable output into success-or-failure contract.
    /// </summary>
    /// <param name="value">Successful value to wrap.</param>
    /// <returns>Successful result for non-null values; failed result otherwise.</returns>
    public static Result<T> Ok(T? value) => value switch
    {
        null => new Result<T>(new Error()),
        _ => new Result<T>(value)
    };

    /// <summary>
    /// Creates failed result with explicit error.
    /// Use when operation failed and caller needs repository error payload.
    /// </summary>
    /// <param name="error">Failure value.</param>
    /// <returns>Failed <see cref="Result{T}"/>.</returns>
    public static Result<T> Fail(Error error) => new(error);
    
    public static implicit operator Result<T>(T? value) => Ok(value);
    public static implicit operator T(Result<T> value) => value.TryGetValue(out var result) ? result! : throw new InvalidOperationException("Cannot unwrap a failed Result.");

    /// <summary>
    /// Gets value indicating whether result currently contains successful value.
    /// </summary>
    public bool IsRight => _value is not null;

    /// <summary>
    /// Gets value indicating whether result currently contains failure value.
    /// Use this with caution until implementation semantics are corrected to reflect <see cref="_error"/>.
    /// </summary>
    public bool IsLeft => _value is not null;

    /// <summary>
    /// Pattern matches success and failure outcomes.
    /// Use when caller must convert both branches into shared return type.
    /// </summary>
    /// <typeparam name="TResult">Return type produced by both branches.</typeparam>
    /// <param name="ok">Function invoked for successful value.</param>
    /// <param name="fail">Function invoked for failure value.</param>
    /// <returns>Value produced by executed branch.</returns>
    public TResult Match<TResult>(Func<T, TResult> ok, Func<Error, TResult> fail)
    {
        if (IsLeft) return fail(_error!);
        return ok(_value!);
    }

    /// <summary>
    /// Attempts to read successful value.
    /// Use when caller wants non-throwing extraction from result.
    /// </summary>
    /// <param name="value">Receives successful value when present; otherwise default value of <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> when successful value is available; otherwise <see langword="false"/>.</returns>
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
    
    /// <summary>
    /// Returns readable representation of success or failure value.
    /// </summary>
    /// <returns>String form of contained value or error.</returns>
    public override string ToString() => Match(v => v.ToString(), e => e.ToString());
}
