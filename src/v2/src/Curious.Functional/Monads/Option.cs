namespace Curious.Functional.Monads;

/// <summary>
/// Represents an optional value that may be present or absent.
/// Use <see cref="Option{T}"/> when a missing value is an expected domain outcome and no explanation for the absence is needed.
/// Use <c>Result&lt;T&gt;</c> or <c>Either&lt;L, R&gt;</c> when the caller needs failure details.
/// </summary>
/// <typeparam name="T">The type of the contained value.</typeparam>
/// <remarks>
/// <para>
/// The <see cref="Option{T}"/> type is a functional programming construct that encapsulates the presence or absence of a value. 
/// It provides a type-safe way to handle optionality, discouraging the use of <see langword="null"/> and the 
/// accompanying <see cref="NullReferenceException"/>.
/// </para>
/// <example>
/// Basic usage:
/// <code>
/// Option&lt;string&gt; name = Option&lt;string&gt;.Some("John");
/// 
/// // Use Match to extract the value or provide a default
/// string greeting = name.Match(
///     some: n => $"Hello, {n}!",
///     none: () => "Hello, stranger!"
/// );
/// 
/// // Use Map to transform the value if it exists
/// Option&lt;int&gt; length = name.Map(n => n.Length);
/// </code>
/// </example>
/// </remarks>
public readonly record struct Option<T> where T : notnull
{
    private readonly T? _value = default;
    private readonly bool _hasValue = false;

    private Option(T value)
    {
        _value = value;
        _hasValue = true;
    }

    /// <summary>Gets the empty option. Use when a value is intentionally absent.</summary>
    public static Option<T> None => new();

    /// <summary>
    /// Creates an option containing <paramref name="value"/>, or <see cref="None"/> if the value is <see langword="null"/>.
    /// </summary>
    public static Option<T> Some(T? value) => value is not null ? new Option<T>(value) : None;

    /// <summary>Implicitly wraps a non-null value in <see cref="Some"/>.</summary>
    public static implicit operator Option<T>(T value) => Some(value);

    /// <summary>
    /// Gets a value indicating whether this option contains a value.
    /// Prefer <see cref="Match{TResult}"/> or <see cref="IfSome"/> over direct branching where possible.
    /// </summary>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Attempts to read the contained value without throwing.
    /// </summary>
    /// <param name="value">Receives the contained value when present; otherwise the default for <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> when a value exists; otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(out T? value)
    {
        value = _value;
        return _hasValue;
    }

    /// <summary>
    /// Transforms the contained value using <paramref name="mapper"/> when present.
    /// Absence is preserved as <see cref="Option{TResult}.None"/>.
    /// </summary>
    public Option<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull =>
        _hasValue ? Option<TResult>.Some(mapper(_value!)) : Option<TResult>.None;

    /// <summary>
    /// Chains an option-returning <paramref name="binder"/> when a value is present, flattening the result.
    /// Use when the next step in a pipeline can itself produce an absent value.
    /// </summary>
    public Option<TResult> Bind<TResult>(Func<T, Option<TResult>> binder) where TResult : notnull =>
        _hasValue ? binder(_value!) : Option<TResult>.None;

    /// <summary>
    /// Evaluates one of two branches depending on whether a value is present.
    /// </summary>
    /// <param name="some">Invoked with the contained value when present.</param>
    /// <param name="none">Invoked when absent.</param>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none) =>
        _hasValue ? some(_value!) : none();

    /// <summary>Executes one of two actions depending on whether a value is present.</summary>
    public Unit Match(Action<T> some, Action none)
    {
        if (_hasValue)
            some(_value!);
        else
            none();

        return Unit.Default;
    }

    /// <summary>
    /// Returns the contained value.
    /// Only use when absence indicates a bug or invalid control flow.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the option is empty.</exception>
    public T Unwrap() =>
        _hasValue ? _value! : throw new InvalidOperationException("Cannot unwrap an empty Option.");

    /// <summary>Returns the contained value, or <paramref name="fallback"/> when absent.</summary>
    public T UnwrapOr(T fallback) => _hasValue ? _value! : fallback;

    /// <summary>Returns the contained value, or lazily computes a fallback when absent.</summary>
    public T UnwrapOrElse(Func<T> fallback) => _hasValue ? _value! : fallback();

    /// <summary>Executes <paramref name="action"/> when absent. Use for side effects such as logging or metrics.</summary>
    public Unit IfNone(Action action)
    {
        if (!_hasValue)
            action();

        return Unit.Default;
    }

    /// <summary>Executes <paramref name="action"/> with the contained value when present. Use for side effects without manual unwrapping.</summary>
    public Unit IfSome(Action<T> action)
    {
        if (_hasValue)
            action(_value!);

        return Unit.Default;
    }

    /// <summary>Returns <c>Some(value)</c> when a value exists; otherwise <c>None</c>.</summary>
    public override string ToString() => _hasValue ? $"Some({_value})" : "None";
}
