namespace CuriousInc.Common.Functional.Monads;

/// <summary>
/// Represents value that can be one of two typed cases.
/// Use <see cref="Either{L,R}"/> when both branches carry meaningful data and branch meaning should stay explicit, such as <c>Left = validation error</c> and <c>Right = success value</c>.
/// Use <see cref="Result{T}"/> instead when left side should always be <see cref="Error"/>.
/// </summary>
/// <typeparam name="L">Left branch type, commonly error or alternate outcome.</typeparam>
/// <typeparam name="R">Right branch type, commonly success value.</typeparam>
public readonly record struct Either<L, R>
{
    private readonly L? _left;
    private readonly R? _value;
    
    private Either(L left) => _left = left;
    private Either(R value) => _value = value;

    /// <summary>
    /// Creates right value.
    /// Use for success branch or preferred branch by convention.
    /// </summary>
    /// <param name="value">Right branch value.</param>
    /// <returns>Right <see cref="Either{L,R}"/>.</returns>
    public static Either<L, R> Right(R value) => new(value);

    /// <summary>
    /// Creates left value.
    /// Use for failure branch or alternate branch by convention.
    /// </summary>
    /// <param name="left">Left branch value.</param>
    /// <returns>Left <see cref="Either{L,R}"/>.</returns>
    public static Either<L, R> Left(L left) => new(left);

    public static implicit operator Either<L, R>(R value) => Right(value);
    public static implicit operator Either<L, R>(L left) => Left(left);
    
    /// <summary>
    /// Gets value indicating whether instance currently holds right branch.
    /// </summary>
    public bool IsRight => _left is null;

    /// <summary>
    /// Gets value indicating whether instance currently holds left branch.
    /// </summary>
    public bool IsLeft => _left is not null;

    /// <summary>
    /// Attempts to read right branch value.
    /// Use when caller wants non-throwing access and can ignore left details.
    /// </summary>
    /// <param name="value">Receives right value when present; otherwise default value of <typeparamref name="R"/>.</param>
    /// <returns><see langword="true"/> when right branch is present; otherwise <see langword="false"/>.</returns>
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

    /// <summary>
    /// Executes <paramref name="action"/> when instance holds right branch.
    /// Use for side effects that should not run for left branch.
    /// </summary>
    /// <param name="action">Action executed for right value.</param>
    public void Apply(Action<R> action)
    {
        if (IsRight && _value is not null)
        {
            action(_value);
        }
    }

    /// <summary>
    /// Pattern matches both branches of value.
    /// Use when caller must handle left and right explicitly.
    /// </summary>
    /// <typeparam name="TResult">Common result type returned by both branches.</typeparam>
    /// <param name="right">Function invoked for right branch.</param>
    /// <param name="left">Function invoked for left branch.</param>
    /// <returns>Value produced by executed branch.</returns>
    public TResult Match<TResult>(Func<R, TResult> right, Func<L, TResult> left) =>
        IsRight && _value is not null
            ? right(_value)
            : left(_left!);

    

    /// <summary>
    /// Returns right branch value.
    /// Use only when left branch would indicate bug or impossible control path.
    /// </summary>
    /// <returns>Right branch value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when instance contains left branch.</exception>
    public R Unwrap()
    {
        if (IsRight && _value is not null)
            return _value;
        throw new InvalidOperationException($"Tried to unwrap an Left result: {_left}");
    }
    
    /// <summary>
    /// Returns left branch value.
    /// Use when caller intentionally expects left branch and wants to assert it.
    /// </summary>
    /// <returns>Left branch value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when instance does not contain left branch.</exception>
    public L Unwrapleft() => (IsLeft ? _left : throw new InvalidOperationException("Tried to unwrap a missing left result."))!;

    /// <summary>
    /// Returns right branch value or maps left branch into fallback value.
    /// Use when caller needs concrete <typeparamref name="R"/> and can recover from left branch.
    /// </summary>
    /// <param name="orElse">Fallback mapper invoked when instance contains left branch.</param>
    /// <returns>Right branch value or fallback value.</returns>
    public R UnwrapOrElse(Func<L, R> orElse)
    {
        return IsRight && _value is not null ? _value : orElse(_left!);
    }

    /// <summary>
    /// Chains right branch into another <see cref="Either{L,R}"/> while preserving left branch type.
    /// Use when next operation can also fail with same left type.
    /// </summary>
    /// <typeparam name="U">Right type returned by chained operation.</typeparam>
    /// <param name="func">Function invoked only for right branch.</param>
    /// <returns>Result of chained operation or original left branch.</returns>
    public Either<L,U> Bind<U>(Func<R, Either<L,U>> func)
        where U : notnull
    {
        return IsRight && _value is not null
            ? func(_value)
            : Either<L,U>.Left(_left!);
    }

    /// <summary>
    /// Transforms right branch into another right type while preserving left branch unchanged.
    /// Use for pure success-path projections.
    /// </summary>
    /// <typeparam name="U">Mapped right type.</typeparam>
    /// <param name="func">Transformation applied only for right branch.</param>
    /// <returns>Mapped right branch or original left branch.</returns>
    public Either<L,U> Map<U>(Func<R, U> func)
        where U : notnull
    {
        return IsRight && _value is not null
            ? Either<L,U>.Right(func(_value))
            : Either<L,U>.Left(_left!);
    }
    
    /// <summary>
    /// Returns readable representation of current branch.
    /// </summary>
    /// <returns><c>Right(...)</c> or <c>Left(...)</c>.</returns>
    public override string ToString() => IsRight ? $"Right({_value})" : $"Left({_left})";
}
