namespace Curious.Functional.Monads;

/// <summary>
/// Represents a value that can be one of two typed cases.
/// Use <see cref="Either{L,R}"/> when both branches carry meaningful data and branch meaning should stay explicit,
/// such as <c>Left = validation error</c> and <c>Right = success value</c>.
/// Use <c>Result&lt;T&gt;</c> when the left side should always be an error type.
/// </summary>
/// <typeparam name="L">Left branch type, commonly an error or alternate outcome.</typeparam>
/// <typeparam name="R">Right branch type, commonly the success value.</typeparam>
/// <remarks>
/// <example>
/// Basic usage:
/// <code>
/// Either&lt;string, int&gt; parsed = int.TryParse(input, out var n)
///     ? Either&lt;string, int&gt;.Right(n)
///     : Either&lt;string, int&gt;.Left("Not a valid number");
///
/// string message = parsed.Match(
///     right: n  => $"Parsed: {n}",
///     left:  err => $"Error: {err}"
/// );
/// </code>
/// </example>
/// </remarks>
public readonly record struct Either<L, R>
    where L : notnull
    where R : notnull
{
    private readonly L? _left = default;
    private readonly R? _right = default;
    private readonly bool _isRight = false;

    private Either(R right)
    {
        _right = right;
        _isRight = true;
    }

    private Either(L left)
    {
        _left = left;
        _isRight = false;
    }

    /// <summary>Creates a right-branch value. Use for the success or preferred branch by convention.</summary>
    public static Either<L, R> Right(R value) => new(value);

    /// <summary>Creates a left-branch value. Use for the failure or alternate branch by convention.</summary>
    public static Either<L, R> Left(L left) => new(left);

    /// <summary>Implicitly wraps a right value.</summary>
    public static implicit operator Either<L, R>(R value) => Right(value);

    /// <summary>Implicitly wraps a left value.</summary>
    public static implicit operator Either<L, R>(L left) => Left(left);

    /// <summary>Gets a value indicating whether this instance holds the right branch.</summary>
    public bool IsRight => _isRight;

    /// <summary>Gets a value indicating whether this instance holds the left branch.</summary>
    public bool IsLeft => !_isRight;

    /// <summary>
    /// Attempts to read the right branch value without throwing.
    /// </summary>
    /// <param name="value">Receives the right value when present; otherwise the default for <typeparamref name="R"/>.</param>
    /// <returns><see langword="true"/> when the right branch is present; otherwise <see langword="false"/>.</returns>
    public bool TryGetValue(out R? value)
    {
        value = _right;
        return _isRight;
    }

    /// <summary>
    /// Evaluates one of two branches depending on which side is active.
    /// </summary>
    /// <param name="right">Invoked with the right value when the right branch is active.</param>
    /// <param name="left">Invoked with the left value when the left branch is active.</param>
    public TResult Match<TResult>(Func<R, TResult> right, Func<L, TResult> left) =>
        _isRight ? right(_right!) : left(_left!);

    /// <summary>Executes one of two actions depending on which side is active.</summary>
    public Unit Match(Action<R> right, Action<L> left)
    {
        if (_isRight)
            right(_right!);
        else
            left(_left!);

        return Unit.Default;
    }

    /// <summary>
    /// Transforms the right value using <paramref name="mapper"/>, preserving the left branch unchanged.
    /// Use for pure success-path projections.
    /// </summary>
    public Either<L, TResult> Map<TResult>(Func<R, TResult> mapper)
        where TResult : notnull =>
        _isRight
            ? Either<L, TResult>.Right(mapper(_right!))
            : Either<L, TResult>.Left(_left!);

    /// <summary>
    /// Transforms the left value using <paramref name="mapper"/>, preserving the right branch unchanged.
    /// Use to adapt the left type without touching the success path.
    /// </summary>
    public Either<TLeft, R> MapLeft<TLeft>(Func<L, TLeft> mapper)
        where TLeft : notnull =>
        _isRight
            ? Either<TLeft, R>.Right(_right!)
            : Either<TLeft, R>.Left(mapper(_left!));

    /// <summary>
    /// Chains the right branch into another <see cref="Either{L,TResult}"/>, preserving the left branch type.
    /// Use when the next step can itself fail with the same left type.
    /// </summary>
    public Either<L, TResult> Bind<TResult>(Func<R, Either<L, TResult>> binder)
        where TResult : notnull =>
        _isRight ? binder(_right!) : Either<L, TResult>.Left(_left!);

    /// <summary>
    /// Returns the right value.
    /// Only use when a left branch would indicate a bug or invalid control flow.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the left branch is active.</exception>
    public R Unwrap() =>
        _isRight ? _right! : throw new InvalidOperationException($"Cannot unwrap a Left value: {_left}");

    /// <summary>
    /// Returns the left value.
    /// Only use when a right branch would indicate a bug or invalid control flow.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the right branch is active.</exception>
    public L UnwrapLeft() =>
        !_isRight ? _left! : throw new InvalidOperationException("Cannot unwrap a Left value from a Right Either.");

    /// <summary>Returns the right value, or maps the left value into a fallback when the left branch is active.</summary>
    public R UnwrapOrElse(Func<L, R> fallback) =>
        _isRight ? _right! : fallback(_left!);

    /// <summary>Executes <paramref name="action"/> with the right value when the right branch is active. Use for side effects.</summary>
    public Unit IfRight(Action<R> action)
    {
        if (_isRight)
            action(_right!);

        return Unit.Default;
    }

    /// <summary>Executes <paramref name="action"/> with the left value when the left branch is active. Use for side effects.</summary>
    public Unit IfLeft(Action<L> action)
    {
        if (!_isRight)
            action(_left!);

        return Unit.Default;
    }

    /// <summary>Returns <c>Right(...)</c> or <c>Left(...)</c>.</summary>
    public override string ToString() => _isRight ? $"Right({_right})" : $"Left({_left})";
}
