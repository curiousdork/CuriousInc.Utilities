using Curious.Functional.ValueObjects;

namespace Curious.Functional.Monads;

/// <summary>
/// Represents the outcome of an operation that either succeeds with a value of type <typeparamref name="T"/>
/// or fails with an <see cref="Error"/>.
/// Use <see cref="Result{T}"/> when failure details matter and success produces a value.
/// Use <see cref="Option{T}"/> when absence carries no explanation.
/// Use <see cref="Either{L,R}"/> when both branches carry meaningful typed data.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <remarks>
/// <example>
/// Basic usage:
/// <code>
/// Result&lt;int&gt; result = int.TryParse(input, out var n)
///     ? Result&lt;int&gt;.Ok(n)
///     : Result&lt;int&gt;.Fail(new Error());
///
/// string message = result.Match(
///     ok:   n  => $"Parsed: {n}",
///     fail: e  => $"Error: {e.Message}"
/// );
/// </code>
/// </example>
/// </remarks>
public readonly record struct Result<T> where T : notnull
{
    private readonly T? _value = default;
    private readonly Error? _error = null;
    private readonly bool _isOk = false;

    private Result(T value)
    {
        _value = value;
        _isOk = true;
    }

    private Result(Error error)
    {
        _error = error;
        _isOk = false;
    }

    /// <summary>Creates a successful result containing <paramref name="value"/>.</summary>
    public static Result<T> Ok(T value) => new(value);

    /// <summary>Creates a failed result carrying <paramref name="error"/>.</summary>
    public static Result<T> Fail(Error error) => new(error);

    /// <summary>Implicitly wraps a value in <see cref="Ok"/>.</summary>
    public static implicit operator Result<T>(T value) => Ok(value);

    /// <summary>Implicitly wraps an error in <see cref="Fail"/>.</summary>
    public static implicit operator Result<T>(Error error) => Fail(error);

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsOk => _isOk;

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsError => !_isOk;

    /// <summary>
    /// Attempts to read the success value without throwing.
    /// </summary>
    /// <param name="value">Receives the success value when present; otherwise the default for <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> when the result is successful; otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(out T? value)
    {
        value = _value;
        return _isOk;
    }

    /// <summary>
    /// Evaluates one of two branches depending on whether the result is a success or failure.
    /// </summary>
    /// <param name="ok">Invoked with the success value when successful.</param>
    /// <param name="fail">Invoked with the error when failed.</param>
    public TResult Match<TResult>(Func<T, TResult> ok, Func<Error, TResult> fail) =>
        _isOk ? ok(_value!) : fail(_error!);

    /// <summary>Executes one of two actions depending on whether the result is a success or failure.</summary>
    public Unit Match(Action<T> ok, Action<Error> fail)
    {
        if (_isOk)
            ok(_value!);
        else
            fail(_error!);

        return Unit.Default;
    }

    /// <summary>
    /// Transforms the success value using <paramref name="mapper"/>.
    /// A failed result propagates its error unchanged.
    /// </summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull =>
        _isOk ? Result<TResult>.Ok(mapper(_value!)) : Result<TResult>.Fail(_error!);

    /// <summary>
    /// Chains a result-returning <paramref name="binder"/> when successful, flattening the outcome.
    /// Use when the next step in a pipeline can itself fail.
    /// </summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder) where TResult : notnull =>
        _isOk ? binder(_value!) : Result<TResult>.Fail(_error!);

    /// <summary>
    /// Returns the success value.
    /// Only use when failure would indicate a bug or invalid control flow.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a failure.</exception>
    public T Unwrap() =>
        _isOk ? _value! : throw new InvalidOperationException($"Cannot unwrap a failed Result: {_error}");

    /// <summary>Returns the success value, or <paramref name="fallback"/> when failed.</summary>
    public T UnwrapOr(T fallback) => _isOk ? _value! : fallback;

    /// <summary>Returns the success value, or lazily computes a fallback when failed.</summary>
    public T UnwrapOrElse(Func<T> fallback) => _isOk ? _value! : fallback();

    /// <summary>Returns the success value, or maps the error into a fallback when failed.</summary>
    public T UnwrapOrElse(Func<Error, T> fallback) => _isOk ? _value! : fallback(_error!);

    /// <summary>Executes <paramref name="action"/> with the success value when successful. Use for side effects.</summary>
    public Unit IfOk(Action<T> action)
    {
        if (_isOk)
            action(_value!);

        return Unit.Default;
    }

    /// <summary>Executes <paramref name="action"/> with the error when failed. Use for side effects such as logging.</summary>
    public Unit IfFail(Action<Error> action)
    {
        if (!_isOk)
            action(_error!);

        return Unit.Default;
    }

    /// <summary>Returns <c>Ok(value)</c> or <c>Fail(error)</c>.</summary>
    public override string ToString() => _isOk ? $"Ok({_value})" : $"Fail({_error})";
}
