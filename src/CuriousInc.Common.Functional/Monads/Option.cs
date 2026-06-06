namespace CuriousInc.Common.Functional.Monads;

/// <summary>
/// Represents optional value that may be present or absent.
/// Use <see cref="Option{T}"/> when missing value is expected domain outcome and you do not need to explain why value is missing.
/// Use <see cref="Result{T}"/> or <see cref="Either{L,R}"/> instead when caller needs failure details.
/// </summary>
/// <typeparam name="T">Type of optional value.</typeparam>
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

    /// <summary>
    /// Gets empty option.
    /// Use when value is intentionally absent.
    /// </summary>
    public static Option<T> None => new();

    /// <summary>
    /// Creates option that contains <paramref name="value"/> when value is not <see langword="null"/>, otherwise returns <see cref="None"/>.
    /// Use when converting nullable reference or value into optional value.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    /// <returns><see cref="Option{T}"/> containing value or <see cref="None"/>.</returns>
    public static Option<T> Some(T? value) => value is not null ? new Option<T>(value) : None;
    
    /// <summary>
    /// Gets value indicating whether option currently contains value.
    /// Use for branching when <see cref="Match{TResult}"/> or <see cref="IfSome"/> is not convenient.
    /// </summary>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Attempts to read contained value.
    /// Use when you want non-throwing extraction.
    /// </summary>
    /// <param name="value">Receives contained value when present; otherwise default value of <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> when value exists; otherwise <see langword="false"/>.</returns>
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
    
    /// <summary>
    /// Maps contained value into another optional value type when value is present.
    /// Use for pure transformations that should preserve absence as absence.
    /// </summary>
    /// <typeparam name="TResult">Mapped value type.</typeparam>
    /// <param name="mapper">Transformation applied only when value exists.</param>
    /// <returns>Mapped option when value exists; otherwise <see cref="Option{TResult}.None"/>.</returns>
    public Option<TResult> Apply<TResult>(Func<T, TResult> mapper) where TResult : notnull
    {
        return _hasValue ? Option<TResult>.Some(mapper(_value!)) : Option<TResult>.None;
    }

    /// <summary>
    /// Pattern matches both possible states of option.
    /// Use when caller must handle both present and absent cases explicitly.
    /// </summary>
    /// <typeparam name="TResult">Return type produced by both branches.</typeparam>
    /// <param name="some">Function invoked when value exists.</param>
    /// <param name="none">Function invoked when value is absent.</param>
    /// <returns>Value returned by executed branch.</returns>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        return _hasValue ? some(_value!) : none();
    }

    /// <summary>
    /// Returns contained value.
    /// Use only when absence would indicate bug or invalid control flow.
    /// </summary>
    /// <returns>Contained value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when option is empty.</exception>
    public T Unwrap()
    {
        if (!_hasValue)
            throw new InvalidOperationException("No value present in Option.");
        return _value!;
    }

    /// <summary>
    /// Returns contained value or computes fallback when value is absent.
    /// Use when you need concrete value and can supply lazy default.
    /// </summary>
    /// <param name="fallback">Function invoked only when option is empty.</param>
    /// <returns>Contained value or fallback value.</returns>
    public T UnwrapOrElse(Func<T> fallback)
    {
        return _hasValue ? _value! : fallback();
    }

    /// <summary>
    /// Executes <paramref name="action"/> when option is empty.
    /// Use for side effects such as logging, metrics, or fallback triggers.
    /// </summary>
    /// <param name="action">Action to execute when value is absent.</param>
    /// <returns><see cref="Unit.Default"/>.</returns>
    public Unit IfNone(Action action)
    {
        if (!_hasValue)
        {
            action();
        }
        
        return Unit.Default;
    }

    /// <summary>
    /// Executes <paramref name="action"/> when option contains value.
    /// Use for side effects without unwrapping manually.
    /// </summary>
    /// <param name="action">Action to execute when value exists.</param>
    /// <returns><see cref="Unit.Default"/>.</returns>
    public Unit IfSome(Action<T> action)
    {
        if (_hasValue)
        {
            action(_value!);
        }
        
        return Unit.Default;
    }

    /// <summary>
    /// Returns readable representation of option state.
    /// </summary>
    /// <returns><c>Some(...)</c> when value exists; otherwise <c>None</c>.</returns>
    public override string ToString()
    {
        return Match(v => $"Some({v})", () => "None");
    }
}
