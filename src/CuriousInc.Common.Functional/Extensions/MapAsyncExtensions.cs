using System.Runtime.CompilerServices;
using CuriousInc.Common.Functional.Monads;

namespace CuriousInc.Common.Functional.Extensions;

/// <summary>
/// Maps values and async values into new async results.
/// </summary>
public static class MapAsyncExtensions
{
    private static CancellationToken ResolveToken(CancellationToken? cancellationToken) =>
        cancellationToken ?? CancellationToken.None;

    [OverloadResolutionPriority(-1)]
    public static Task<TResult> MapAsync<T, TResult>(
        this T value,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return mapper(value, ct);
    }

    [OverloadResolutionPriority(-2)]
    public static Task<TResult> MapAsync<T, TResult>(
        this T value,
        Func<T, CancellationToken, TResult> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(mapper(value, ct));
    }

    [OverloadResolutionPriority(-2)]
    public static Task<TResult> MapAsync<T, TResult>(
        this T value,
        Func<T, TResult> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(mapper(value));
    }

    [OverloadResolutionPriority(-1)]
    public static async Task<TResult> MapAsync<T, TResult>(
        this Task<T> task,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return await mapper(value, ct).ConfigureAwait(false);
    }

    [OverloadResolutionPriority(-2)]
    public static async Task<TResult> MapAsync<T, TResult>(
        this Task<T> task,
        Func<T, CancellationToken, TResult> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return mapper(value, ct);
    }

    [OverloadResolutionPriority(-2)]
    public static async Task<TResult> MapAsync<T, TResult>(
        this Task<T> task,
        Func<T, TResult> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return mapper(value);
    }

    [OverloadResolutionPriority(-1)]
    public static async ValueTask<TResult> MapAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return await mapper(value, ct).ConfigureAwait(false);
    }

    [OverloadResolutionPriority(-2)]
    public static async ValueTask<TResult> MapAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, CancellationToken, TResult> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return mapper(value, ct);
    }

    [OverloadResolutionPriority(-2)]
    public static async ValueTask<TResult> MapAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, TResult> mapper,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return mapper(value);
    }

    public static async IAsyncEnumerable<TResult> MapAsync<T, TResult>(
        this IAsyncEnumerable<T> source,
        Func<T, CancellationToken, Task<TResult>> mapper,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return await mapper(item, cancellationToken).ConfigureAwait(false);
        }
    }

    public static Task<Option<TResult>> MapAsync<T, TResult>(
        this Option<T> option,
        Func<T, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where TResult : notnull =>
        option.MapAsync((value, _) => mapper(value), cancellationToken);

    public static Task<Option<TResult>> MapAsync<T, TResult>(
        this Option<T> option,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where TResult : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return option.Match(
            async value => Option<TResult>.Some(await mapper(value, ct).ConfigureAwait(false)),
            () => Task.FromResult(Option<TResult>.None));
    }

    public static Task<Option<TResult>> MapAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where TResult : notnull =>
        optionTask.MapAsync((value, _) => mapper(value), cancellationToken);

    public static async Task<Option<TResult>> MapAsync<T, TResult>(
        this Task<Option<T>> optionTask,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where TResult : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var option = await optionTask.ConfigureAwait(false);
        return await option.MapAsync(mapper, ct).ConfigureAwait(false);
    }

    public static Task<Either<L, U>> MapAsync<L, R, U>(
        this Either<L, R> either,
        Func<R, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        either.MapAsync((value, _) => mapper(value), cancellationToken);

    public static Task<Either<L, U>> MapAsync<L, R, U>(
        this Either<L, R> either,
        Func<R, CancellationToken, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return either.Match(
            async value => Either<L, U>.Right(await mapper(value, ct).ConfigureAwait(false)),
            left => Task.FromResult(Either<L, U>.Left(left)));
    }

    public static Task<Either<L, U>> MapAsync<L, R, U>(
        this Task<Either<L, R>> eitherTask,
        Func<R, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        eitherTask.MapAsync((value, _) => mapper(value), cancellationToken);

    public static async Task<Either<L, U>> MapAsync<L, R, U>(
        this Task<Either<L, R>> eitherTask,
        Func<R, CancellationToken, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var either = await eitherTask.ConfigureAwait(false);
        return await either.MapAsync(mapper, ct).ConfigureAwait(false);
    }

    public static Task<Result<U>> MapAsync<T, U>(
        this Result<T> result,
        Func<T, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        result.MapAsync((value, _) => mapper(value), cancellationToken);

    public static Task<Result<U>> MapAsync<T, U>(
        this Result<T> result,
        Func<T, CancellationToken, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return result.Match(
            async value => Result<U>.Ok(await mapper(value, ct).ConfigureAwait(false)),
            error => Task.FromResult(Result<U>.Fail(error)));
    }

    public static Task<Result<U>> MapAsync<T, U>(
        this Task<Result<T>> resultTask,
        Func<T, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull =>
        resultTask.MapAsync((value, _) => mapper(value), cancellationToken);

    public static async Task<Result<U>> MapAsync<T, U>(
        this Task<Result<T>> resultTask,
        Func<T, CancellationToken, Task<U>> mapper,
        CancellationToken? cancellationToken = null)
        where U : notnull
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var result = await resultTask.ConfigureAwait(false);
        return await result.MapAsync(mapper, ct).ConfigureAwait(false);
    }

    public static Task<Union<T1, T2>> MapAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T1, Task<Union<T1, T2>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2>> MapAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T1, CancellationToken, Task<Union<T1, T2>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2>> MapAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T2, Task<Union<T1, T2>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2>> MapAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T2, CancellationToken, Task<Union<T1, T2>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2>> ApplyUnionAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T1, Task<Union<T1, T2>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2>> ApplyUnionAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T1, CancellationToken, Task<Union<T1, T2>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2>>(value));
    }

    public static Task<Union<T1, T2>> ApplyUnionAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T2, Task<Union<T1, T2>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2>> ApplyUnionAsync<T1, T2>(
        this Union<T1, T2> union,
        Func<T2, CancellationToken, Task<Union<T1, T2>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2>>(value),
            value => functor(value, ct));
    }

    public static Task<Union<T1, T2, T3>> MapAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T1, Task<Union<T1, T2, T3>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3>> MapAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3>> MapAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T2, Task<Union<T1, T2, T3>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3>> MapAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3>> MapAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T3, Task<Union<T1, T2, T3>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3>> MapAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T1, Task<Union<T1, T2, T3>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            value => Task.FromResult<Union<T1, T2, T3>>(value));
    }

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T2, Task<Union<T1, T2, T3>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3>>(value));
    }

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T3, Task<Union<T1, T2, T3>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3>> ApplyUnionAsync<T1, T2, T3>(
        this Union<T1, T2, T3> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            value => Task.FromResult<Union<T1, T2, T3>>(value),
            value => functor(value, ct));
    }

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T2, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T3, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T4, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> MapAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T4, CancellationToken, Task<Union<T1, T2, T3, T4>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value));
    }

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T2, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value));
    }

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T3, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value));
    }

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T4, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4>> ApplyUnionAsync<T1, T2, T3, T4>(
        this Union<T1, T2, T3, T4> union,
        Func<T4, CancellationToken, Task<Union<T1, T2, T3, T4>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4>>(value),
            value => functor(value, ct));
    }

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T2, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T3, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T4, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T4, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T5, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> MapAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T5, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T2, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T3, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T4, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T4, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T5, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5>> ApplyUnionAsync<T1, T2, T3, T4, T5>(
        this Union<T1, T2, T3, T4, T5> union,
        Func<T5, CancellationToken, Task<Union<T1, T2, T3, T4, T5>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5>>(value),
            value => functor(value, ct));
    }

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T2, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T3, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T4, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T4, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T5, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T5, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T6, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> MapAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T6, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> mapper,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync(mapper, cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T1, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T2, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T2, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T3, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T3, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T4, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T4, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T5, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T5, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => functor(value, ct),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value));
    }

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T6, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null) =>
        union.ApplyUnionAsync((value, _) => functor(value), cancellationToken);

    public static Task<Union<T1, T2, T3, T4, T5, T6>> ApplyUnionAsync<T1, T2, T3, T4, T5, T6>(
        this Union<T1, T2, T3, T4, T5, T6> union,
        Func<T6, CancellationToken, Task<Union<T1, T2, T3, T4, T5, T6>>> functor,
        CancellationToken? cancellationToken = null)
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return union.Match(
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => Task.FromResult<Union<T1, T2, T3, T4, T5, T6>>(value),
            value => functor(value, ct));
    }
}
