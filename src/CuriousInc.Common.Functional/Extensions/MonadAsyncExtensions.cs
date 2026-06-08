namespace CuriousInc.Common.Functional.Extensions;

using CuriousInc.Common.Functional.Monads;

/// <summary>
/// Provides async-friendly extension members for functional monads and unions.
/// Use these helpers when branch handlers or projections must await asynchronous work without unwrapping manually.
/// </summary>
public static class MonadAsyncExtensions
{
    private static CancellationToken ResolveToken(CancellationToken? cancellationToken) =>
        cancellationToken ?? CancellationToken.None;

    public static Task<TResult> MatchAsync<T, TResult>(
        this Option<T> option,
        Func<T, Task<TResult>> some,
        Func<Task<TResult>> none,
        CancellationToken? cancellationToken = null) =>
        option.MatchAsync(
            (value, _) => some(value),
            _ => none(),
            cancellationToken);

    public static Task<TResult> MatchAsync<T, TResult>(
        this Option<T> option,
        Func<T, CancellationToken, Task<TResult>> some,
        Func<CancellationToken, Task<TResult>> none,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return option.Match(
            value => some(value, ct),
            () => none(ct));
    }

    public static Task<TResult> MatchAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, Task<TResult>> some,
        Func<Task<TResult>> none,
        CancellationToken? cancellationToken = null) =>
        optionTask.MatchAsync(
            (value, _) => some(value),
            _ => none(),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, CancellationToken, Task<TResult>> some,
        Func<CancellationToken, Task<TResult>> none,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var option = await optionTask.ConfigureAwait(false);
        return await option.MatchAsync(some, none, ct).ConfigureAwait(false);
    }

    public static Task<Option<TResult>> BindAsync<T, TResult>(
        this Option<T> option,
        Func<T, Task<Option<TResult>>> binder,
        CancellationToken? cancellationToken = null)
        where TResult : notnull =>
        option.BindAsync((value, _) => binder(value), cancellationToken);

    public static Task<Option<TResult>> BindAsync<T, TResult>(
        this Option<T> option,
        Func<T, CancellationToken, Task<Option<TResult>>> binder,
        CancellationToken? cancellationToken = null)
        where TResult : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return option.Match(
            value => binder(value, ct),
            () => Task.FromResult(Option<TResult>.None));
    }

    public static Task<Option<TResult>> BindAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, Task<Option<TResult>>> binder,
        CancellationToken? cancellationToken = null)
        where TResult : notnull =>
        optionTask.BindAsync((value, _) => binder(value), cancellationToken);

    public static async Task<Option<TResult>> BindAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, CancellationToken, Task<Option<TResult>>> binder,
        CancellationToken? cancellationToken = null)
        where TResult : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var option = await optionTask.ConfigureAwait(false);
        return await option.BindAsync(binder, ct).ConfigureAwait(false);
    }

    public static Task<Unit> IfSomeAsync<T>(
        this Option<T> option,
        Func<T, Task> action,
        CancellationToken? cancellationToken = null) =>
        option.IfSomeAsync((value, _) => action(value), cancellationToken);

    public static Task<Unit> IfSomeAsync<T>(
        this Option<T> option,
        Func<T, CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return option.Match(
            async value =>
            {
                await action(value, ct).ConfigureAwait(false);
                return Unit.Default;
            },
            () => Unit.Async);
    }

    public static Task<Unit> IfSomeAsync<T>(
        this Task<Option<T>> optionTask,
        Func<T, Task> action,
        CancellationToken? cancellationToken = null) =>
        optionTask.IfSomeAsync((value, _) => action(value), cancellationToken);

    public static async Task<Unit> IfSomeAsync<T>(
        this Task<Option<T>> optionTask,
        Func<T, CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var option = await optionTask.ConfigureAwait(false);
        return await option.IfSomeAsync(action, ct).ConfigureAwait(false);
    }

    public static Task<Unit> IfNoneAsync<T>(
        this Option<T> option,
        Func<Task> action,
        CancellationToken? cancellationToken = null) =>
        option.IfNoneAsync(_ => action(), cancellationToken);

    public static Task<Unit> IfNoneAsync<T>(
        this Option<T> option,
        Func<CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return option.Match(
            _ => Unit.Async,
            async () =>
            {
                await action(ct).ConfigureAwait(false);
                return Unit.Default;
            });
    }

    public static Task<Unit> IfNoneAsync<T>(
        this Task<Option<T>> optionTask,
        Func<Task> action,
        CancellationToken? cancellationToken = null) =>
        optionTask.IfNoneAsync(_ => action(), cancellationToken);

    public static async Task<Unit> IfNoneAsync<T>(
        this Task<Option<T>> optionTask,
        Func<CancellationToken, Task> action,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var option = await optionTask.ConfigureAwait(false);
        return await option.IfNoneAsync(action, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<L, R, TResult>(
        this Either<L, R> either,
        Func<R, Task<TResult>> right,
        Func<L, Task<TResult>> left,
        CancellationToken? cancellationToken = null) =>
        either.MatchAsync(
            (value, _) => right(value),
            (value, _) => left(value),
            cancellationToken);

    public static Task<TResult> MatchAsync<L, R, TResult>(
        this Either<L, R> either,
        Func<R, CancellationToken, Task<TResult>> right,
        Func<L, CancellationToken, Task<TResult>> left,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return either.Match(
            value => right(value, ct),
            value => left(value, ct));
    }

    public static Task<TResult> MatchAsync<L, R, TResult>(
        this Task<Either<L, R>> eitherTask,
        Func<R, Task<TResult>> right,
        Func<L, Task<TResult>> left,
        CancellationToken? cancellationToken = null) =>
        eitherTask.MatchAsync(
            (value, _) => right(value),
            (value, _) => left(value),
            cancellationToken);

    public static async Task<TResult> MatchAsync<L, R, TResult>(
        this Task<Either<L, R>> eitherTask,
        Func<R, CancellationToken, Task<TResult>> right,
        Func<L, CancellationToken, Task<TResult>> left,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var either = await eitherTask.ConfigureAwait(false);
        return await either.MatchAsync(right, left, ct).ConfigureAwait(false);
    }

    public static Task<Either<L, U>> BindAsync<L, R, U>(
        this Either<L, R> either,
        Func<R, Task<Either<L, U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        either.BindAsync((value, _) => binder(value), cancellationToken);

    public static Task<Either<L, U>> BindAsync<L, R, U>(
        this Either<L, R> either,
        Func<R, CancellationToken, Task<Either<L, U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return either.Match(
            value => binder(value, ct),
            left => Task.FromResult(Either<L, U>.Left(left)));
    }

    public static Task<Either<L, U>> BindAsync<L, R, U>(
        this Task<Either<L, R>> eitherTask,
        Func<R, Task<Either<L, U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        eitherTask.BindAsync((value, _) => binder(value), cancellationToken);

    public static async Task<Either<L, U>> BindAsync<L, R, U>(
        this Task<Either<L, R>> eitherTask,
        Func<R, CancellationToken, Task<Either<L, U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var either = await eitherTask.ConfigureAwait(false);
        return await either.BindAsync(binder, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<T, TResult>(
        this Result<T> result,
        Func<T, Task<TResult>> ok,
        Func<Error, Task<TResult>> fail,
        CancellationToken? cancellationToken = null) =>
        result.MatchAsync(
            (value, _) => ok(value),
            (error, _) => fail(error),
            cancellationToken);

    public static Task<TResult> MatchAsync<T, TResult>(
        this Result<T> result,
        Func<T, CancellationToken, Task<TResult>> ok,
        Func<Error, CancellationToken, Task<TResult>> fail,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return result.Match(
            value => ok(value, ct),
            error => fail(error, ct));
    }

    public static Task<TResult> MatchAsync<T, TResult>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TResult>> ok,
        Func<Error, Task<TResult>> fail,
        CancellationToken? cancellationToken = null) =>
        resultTask.MatchAsync(
            (value, _) => ok(value),
            (error, _) => fail(error),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Result<T>> resultTask,
        Func<T, CancellationToken, Task<TResult>> ok,
        Func<Error, CancellationToken, Task<TResult>> fail,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return await result.MatchAsync(ok, fail, ct).ConfigureAwait(false);
    }

    public static Task<Result<U>> BindAsync<T, U>(
        this Result<T> result,
        Func<T, Task<Result<U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        result.BindAsync((value, _) => binder(value), cancellationToken);

    public static Task<Result<U>> BindAsync<T, U>(
        this Result<T> result,
        Func<T, CancellationToken, Task<Result<U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return result.Match(
            value => binder(value, ct),
            error => Task.FromResult(Result<U>.Fail(error)));
    }

    public static Task<Result<U>> BindAsync<T, U>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        resultTask.BindAsync((value, _) => binder(value), cancellationToken);

    public static async Task<Result<U>> BindAsync<T, U>(
        this Task<Result<T>> resultTask,
        Func<T, CancellationToken, Task<Result<U>>> binder,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return await result.BindAsync(binder, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<T1, T2, TResult>(
        this Union<T1, T2> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        CancellationToken? cancellationToken = null) =>
        union.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            cancellationToken);

    public static Task<TResult> MatchAsync<T1, T2, TResult>(
        this Union<T1, T2> union,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => func1(value, ct),
            value => func2(value, ct));
    }

    public static Task<TResult> MatchAsync<T1, T2, TResult>(
        this Task<Union<T1, T2>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        CancellationToken? cancellationToken = null) =>
        unionTask.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T1, T2, TResult>(
        this Task<Union<T1, T2>> unionTask,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var union = await unionTask.ConfigureAwait(false);
        return await union.MatchAsync(func1, func2, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, TResult>(
        this Union<T1, T2, T3> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        CancellationToken? cancellationToken = null) =>
        union.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            cancellationToken);

    public static Task<TResult> MatchAsync<T1, T2, T3, TResult>(
        this Union<T1, T2, T3> union,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => func1(value, ct),
            value => func2(value, ct),
            value => func3(value, ct));
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, TResult>(
        this Task<Union<T1, T2, T3>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        CancellationToken? cancellationToken = null) =>
        unionTask.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T1, T2, T3, TResult>(
        this Task<Union<T1, T2, T3>> unionTask,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var union = await unionTask.ConfigureAwait(false);
        return await union.MatchAsync(func1, func2, func3, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        CancellationToken? cancellationToken = null) =>
        union.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            (value, _) => func4(value),
            cancellationToken);

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        Func<T4, CancellationToken, Task<TResult>> func4,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => func1(value, ct),
            value => func2(value, ct),
            value => func3(value, ct),
            value => func4(value, ct));
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(
        this Task<Union<T1, T2, T3, T4>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        CancellationToken? cancellationToken = null) =>
        unionTask.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            (value, _) => func4(value),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, TResult>(
        this Task<Union<T1, T2, T3, T4>> unionTask,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        Func<T4, CancellationToken, Task<TResult>> func4,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var union = await unionTask.ConfigureAwait(false);
        return await union.MatchAsync(func1, func2, func3, func4, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5,
        CancellationToken? cancellationToken = null) =>
        union.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            (value, _) => func4(value),
            (value, _) => func5(value),
            cancellationToken);

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        Func<T4, CancellationToken, Task<TResult>> func4,
        Func<T5, CancellationToken, Task<TResult>> func5,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => func1(value, ct),
            value => func2(value, ct),
            value => func3(value, ct),
            value => func4(value, ct),
            value => func5(value, ct));
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(
        this Task<Union<T1, T2, T3, T4, T5>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5,
        CancellationToken? cancellationToken = null) =>
        unionTask.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            (value, _) => func4(value),
            (value, _) => func5(value),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, TResult>(
        this Task<Union<T1, T2, T3, T4, T5>> unionTask,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        Func<T4, CancellationToken, Task<TResult>> func4,
        Func<T5, CancellationToken, Task<TResult>> func5,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var union = await unionTask.ConfigureAwait(false);
        return await union.MatchAsync(func1, func2, func3, func4, func5, ct).ConfigureAwait(false);
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5,
        Func<T6, Task<TResult>> func6,
        CancellationToken? cancellationToken = null) =>
        union.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            (value, _) => func4(value),
            (value, _) => func5(value),
            (value, _) => func6(value),
            cancellationToken);

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        Func<T4, CancellationToken, Task<TResult>> func4,
        Func<T5, CancellationToken, Task<TResult>> func5,
        Func<T6, CancellationToken, Task<TResult>> func6,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => func1(value, ct),
            value => func2(value, ct),
            value => func3(value, ct),
            value => func4(value, ct),
            value => func5(value, ct),
            value => func6(value, ct));
    }

    public static Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this Task<Union<T1, T2, T3, T4, T5, T6>> unionTask,
        Func<T1, Task<TResult>> func1,
        Func<T2, Task<TResult>> func2,
        Func<T3, Task<TResult>> func3,
        Func<T4, Task<TResult>> func4,
        Func<T5, Task<TResult>> func5,
        Func<T6, Task<TResult>> func6,
        CancellationToken? cancellationToken = null) =>
        unionTask.MatchAsync(
            (value, _) => func1(value),
            (value, _) => func2(value),
            (value, _) => func3(value),
            (value, _) => func4(value),
            (value, _) => func5(value),
            (value, _) => func6(value),
            cancellationToken);

    public static async Task<TResult> MatchAsync<T1, T2, T3, T4, T5, T6, TResult>(
        this Task<Union<T1, T2, T3, T4, T5, T6>> unionTask,
        Func<T1, CancellationToken, Task<TResult>> func1,
        Func<T2, CancellationToken, Task<TResult>> func2,
        Func<T3, CancellationToken, Task<TResult>> func3,
        Func<T4, CancellationToken, Task<TResult>> func4,
        Func<T5, CancellationToken, Task<TResult>> func5,
        Func<T6, CancellationToken, Task<TResult>> func6,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var union = await unionTask.ConfigureAwait(false);
        return await union.MatchAsync(func1, func2, func3, func4, func5, func6, ct).ConfigureAwait(false);
    }

}
