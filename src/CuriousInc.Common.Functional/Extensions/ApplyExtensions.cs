using System.Runtime.CompilerServices;

namespace CuriousInc.Common.Functional.Extensions;

public static class ApplyAsyncExtensions
{
    // 1) T  ➜ Func<T, TResult> (sync)            => Task<TResult>
    public static Task<TResult> ApplyAsync<T, TResult>(
        this T value,
        Func<T, TResult> func)
        => Task.FromResult(func(value));

    // 2) T  ➜ Func<T, Task<TResult>> (async)     => Task<TResult>
    public static Task<TResult> ApplyAsync<T, TResult>(
        this T value,
        Func<T, Task<TResult>> func)
        => func(value);

    // 3) T  ➜ Func<T, CancellationToken, Task<TResult>> (async, cancellable) => Task<TResult>
    public static Task<TResult> ApplyAsync<T, TResult>(
        this T value,
        Func<T, CancellationToken, Task<TResult>> func,
        CancellationToken ct)
        => func(value, ct);

    // 4) Task<T>  ➜ Func<T, TResult> (sync)      => Task<TResult>
    public static async Task<TResult> ApplyAsync<T, TResult>(
        this Task<T> task,
        Func<T, TResult> func)
    {
        var value = await task.ConfigureAwait(false);
        return func(value);
    }

    // 5) Task<T>  ➜ Func<T, Task<TResult>> (async) => Task<TResult>
    public static async Task<TResult> ApplyAsync<T, TResult>(
        this Task<T> task,
        Func<T, Task<TResult>> func)
    {
        var value = await task.ConfigureAwait(false);
        return await func(value).ConfigureAwait(false);
    }

    // 6) Task<T>  ➜ Func<T, CancellationToken, Task<TResult>> (async, cancellable)
    public static async Task<TResult> ApplyAsync<T, TResult>(
        this Task<T> task,
        Func<T, CancellationToken, Task<TResult>> func,
        CancellationToken ct)
    {
        var value = await task.ConfigureAwait(false);
        ct.ThrowIfCancellationRequested();
        return await func(value, ct).ConfigureAwait(false);
    }

    // 7) ValueTask<T> variants (sync func)
    public static async ValueTask<TResult> ApplyAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, TResult> func)
    {
        var value = await task.ConfigureAwait(false);
        return func(value);
    }

    // 8) ValueTask<T> variants (async func)
    public static async ValueTask<TResult> ApplyAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, Task<TResult>> func)
    {
        var value = await task.ConfigureAwait(false);
        var result = await func(value).ConfigureAwait(false);
        return result;
    }

    // 9) ValueTask<T> variants (async, cancellable)
    public static async ValueTask<TResult> ApplyAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, CancellationToken, Task<TResult>> func,
        CancellationToken ct)
    {
        var value = await task.ConfigureAwait(false);
        ct.ThrowIfCancellationRequested();
        var result = await func(value, ct).ConfigureAwait(false);
        return result;
    }

    // 10) IAsyncEnumerable<T> ➜ projector (async) => IAsyncEnumerable<TResult>
    // Handy for streaming pipelines.
    public static async IAsyncEnumerable<TResult> ApplyAsync<T, TResult>(
        this IAsyncEnumerable<T> source,
        Func<T, Task<TResult>> projector,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var item in source.WithCancellation(ct).ConfigureAwait(false))
        {
            ct.ThrowIfCancellationRequested();
            yield return await projector(item).ConfigureAwait(false);
        }
    }
}