namespace CuriousInc.Common.Functional.Extensions;

using CuriousInc.Common.Functional.Monads;

/// <summary>
/// Provides async-friendly extension members for functional monads and unions.
/// Use these helpers when branch handlers or projections must await asynchronous work without unwrapping manually.
/// </summary>
public static class MonadAsyncExtensions
{
    public static Task<TResult> MatchAsync<T, TResult>(
        this Option<T> option,
        Func<T, Task<TResult>> some,
        Func<Task<TResult>> none)
        => option.Match(some, none);

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, Task<TResult>> some,
        Func<Task<TResult>> none)
        => await (await optionTask.ConfigureAwait(false)).MatchAsync(some, none).ConfigureAwait(false);

    public static Task<Option<TResult>> MapAsync<T, TResult>(
        this Option<T> option,
        Func<T, Task<TResult>> mapper)
        where TResult : notnull
        => option.Match(
            async value => Option<TResult>.Some(await mapper(value).ConfigureAwait(false)),
            () => Task.FromResult(Option<TResult>.None));

    public static async Task<Option<TResult>> MapAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, Task<TResult>> mapper)
        where TResult : notnull
        => await (await optionTask.ConfigureAwait(false)).MapAsync(mapper).ConfigureAwait(false);

    public static Task<Option<TResult>> BindAsync<T, TResult>(
        this Option<T> option,
        Func<T, Task<Option<TResult>>> binder)
        where TResult : notnull
        => option.Match(
            binder,
            () => Task.FromResult(Option<TResult>.None));

    public static async Task<Option<TResult>> BindAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, Task<Option<TResult>>> binder)
        where TResult : notnull
        => await (await optionTask.ConfigureAwait(false)).BindAsync(binder).ConfigureAwait(false);

    public static Task<Unit> IfSomeAsync<T>(
        this Option<T> option,
        Func<T, Task> action)
        => option.Match(
            async value =>
            {
                await action(value).ConfigureAwait(false);
                return Unit.Default;
            },
            () => Unit.Async);

    public static async Task<Unit> IfSomeAsync<T>(
        this Task<Option<T>> optionTask,
        Func<T, Task> action)
        => await (await optionTask.ConfigureAwait(false)).IfSomeAsync(action).ConfigureAwait(false);

    public static Task<Unit> IfNoneAsync<T>(
        this Option<T> option,
        Func<Task> action)
        => option.Match(
            _ => Unit.Async,
            async () =>
            {
                await action().ConfigureAwait(false);
                return Unit.Default;
            });

    public static async Task<Unit> IfNoneAsync<T>(
        this Task<Option<T>> optionTask,
        Func<Task> action)
        => await (await optionTask.ConfigureAwait(false)).IfNoneAsync(action).ConfigureAwait(false);

    public static Task<TResult> MatchAsync<L, R, TResult>(
        this Either<L, R> either,
        Func<R, Task<TResult>> right,
        Func<L, Task<TResult>> left)
        => either.Match(right, left);

    public static async Task<TResult> MatchAsync<L, R, TResult>(
        this Task<Either<L, R>> eitherTask,
        Func<R, Task<TResult>> right,
        Func<L, Task<TResult>> left)
        => await (await eitherTask.ConfigureAwait(false)).MatchAsync(right, left).ConfigureAwait(false);

    public static Task<Either<L, U>> MapAsync<L, R, U>(
        this Either<L, R> either,
        Func<R, Task<U>> mapper)
        where U : notnull
        => either.Match(
            async value => Either<L, U>.Right(await mapper(value).ConfigureAwait(false)),
            left => Task.FromResult(Either<L, U>.Left(left)));

    public static async Task<Either<L, U>> MapAsync<L, R, U>(
        this Task<Either<L, R>> eitherTask,
        Func<R, Task<U>> mapper)
        where U : notnull
        => await (await eitherTask.ConfigureAwait(false)).MapAsync(mapper).ConfigureAwait(false);

    public static Task<Either<L, U>> BindAsync<L, R, U>(
        this Either<L, R> either,
        Func<R, Task<Either<L, U>>> binder)
        where U : notnull
        => either.Match(
            binder,
            left => Task.FromResult(Either<L, U>.Left(left)));

    public static async Task<Either<L, U>> BindAsync<L, R, U>(
        this Task<Either<L, R>> eitherTask,
        Func<R, Task<Either<L, U>>> binder)
        where U : notnull
        => await (await eitherTask.ConfigureAwait(false)).BindAsync(binder).ConfigureAwait(false);

    public static Task<TResult> MatchAsync<T, TResult>(
        this Result<T> result,
        Func<T, Task<TResult>> ok,
        Func<Error, Task<TResult>> fail)
        => result.Match(ok, fail);

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TResult>> ok,
        Func<Error, Task<TResult>> fail)
        => await (await resultTask.ConfigureAwait(false)).MatchAsync(ok, fail).ConfigureAwait(false);

    public static Task<Result<U>> MapAsync<T, U>(
        this Result<T> result,
        Func<T, Task<U>> mapper)
        where U : notnull
        => result.Match(
            async value => Result<U>.Ok(await mapper(value).ConfigureAwait(false)),
            error => Task.FromResult(Result<U>.Fail(error)));

    public static async Task<Result<U>> MapAsync<T, U>(
        this Task<Result<T>> resultTask,
        Func<T, Task<U>> mapper)
        where U : notnull
        => await (await resultTask.ConfigureAwait(false)).MapAsync(mapper).ConfigureAwait(false);

    public static Task<Result<U>> BindAsync<T, U>(
        this Result<T> result,
        Func<T, Task<Result<U>>> binder)
        where U : notnull
        => result.Match(
            binder,
            error => Task.FromResult(Result<U>.Fail(error)));

    public static async Task<Result<U>> BindAsync<T, U>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<U>>> binder)
        where U : notnull
        => await (await resultTask.ConfigureAwait(false)).BindAsync(binder).ConfigureAwait(false);

    public static Task<TResult> MatchAsync<T1, T2, TResult>(
        this Union<T1, T2> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2)
        => union.Match(func1, func2);

    public static async Task<TResult> MatchAsync<T1, T2, TResult>(
        this Task<Union<T1, T2>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2)
        => await (await unionTask.ConfigureAwait(false)).MatchAsync(func1, func2).ConfigureAwait(false);

    public static Task<Union<T1, T2>> ApplyUnionAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T1, Task<Union<T1, T2>>> functor)
        => union.Match(
            functor,
            value => Task.FromResult<Union<T1, T2>>(value));

    public static Task<Union<T1, T2>> ApplyUnionAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T2, Task<Union<T1, T2>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2>>(value),
            functor);

    public static Task<TResult> MatchAsync<T1, T2, T3, TResult>(
        this Union<T1, T2, T3> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3)
        => union.Match(func1, func2, func3);

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(
        this Task<Union<T1, T2, T3>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3)
        => await (await unionTask.ConfigureAwait(false)).MatchAsync(func1, func2, func3).ConfigureAwait(false);

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T1, Task<Union<T1, T2, T3>>> functor)
        => union.Match(
            functor,
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            value => Task.FromResult<Union<T1, T2, T3>>(value));

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T2, Task<Union<T1, T2, T3>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3>>(value));

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T3, Task<Union<T1, T2, T3>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            functor);

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4)
        => union.Match(func1, func2, func3, func4);

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(
        this Task<Union<T1, T2, T3, T4>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4)
        => await (await unionTask.ConfigureAwait(false)).MatchAsync(func1, func2, func3, func4).ConfigureAwait(false);

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, Task<Union<T1, T2, T3, T4>>> functor)
        => union.Match(
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value));

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T2, Task<Union<T1, T2, T3, T4>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value));

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T3, Task<Union<T1, T2, T3, T4>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value));

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T4, Task<Union<T1, T2, T3, T4>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            functor);

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5)
        => union.Match(func1, func2, func3, func4, func5);

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(
        this Task<Union<T1, T2, T3, T4, T5>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5)
        => await (await unionTask.ConfigureAwait(false)).MatchAsync(func1, func2, func3, func4, func5).ConfigureAwait(false);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, Task<Union<T1, T2, T3, T4, T5>>> functor)
        => union.Match(
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T2, Task<Union<T1, T2, T3, T4, T5>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T3, Task<Union<T1, T2, T3, T4, T5>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T4, Task<Union<T1, T2, T3, T4, T5>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T5, Task<Union<T1, T2, T3, T4, T5>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            functor);

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5,
        Func<T6, Task<TResult>> func6)
        => union.Match(func1, func2, func3, func4, func5, func6);

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this Task<Union<T1, T2, T3, T4, T5, T6>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5,
        Func<T6, Task<TResult>> func6)
        => await (await unionTask.ConfigureAwait(false)).MatchAsync(func1, func2, func3, func4, func5, func6).ConfigureAwait(false);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, Task<Union<T1, T2, T3, T4, T5, T6>>> functor)
        => union.Match(
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T2, Task<Union<T1, T2, T3, T4, T5, T6>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T3, Task<Union<T1, T2, T3, T4, T5, T6>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T4, Task<Union<T1, T2, T3, T4, T5, T6>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T5, Task<Union<T1, T2, T3, T4, T5, T6>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            functor,
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T6, Task<Union<T1, T2, T3, T4, T5, T6>>> functor)
        => union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            functor);
}
