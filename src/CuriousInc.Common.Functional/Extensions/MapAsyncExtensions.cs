using System.Runtime.CompilerServices;

namespace CuriousInc.Common.Functional.Extensions;

/// <summary>
/// Maps values and async values into new async results.
/// </summary>
public static class MapAsyncExtensions
{
    private static CancellationToken ResolveToken(CancellationToken? cancellationToken) =>
        cancellationToken ?? CancellationToken.None;

    public static Task<TResult> MapAsync<T, TResult>(
        this T value,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where T : class
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        return mapper(value, ct);
    }

    public static async Task<TResult> MapAsync<T, TResult>(
        this Task<T> task,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where T : class
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return await mapper(value, ct).ConfigureAwait(false);
    }

    public static async ValueTask<TResult> MapAsync<T, TResult>(
        this ValueTask<T> task,
        Func<T, CancellationToken, Task<TResult>> mapper,
        CancellationToken? cancellationToken = null)
        where T : class
    {
        var ct = ResolveToken(cancellationToken);
        ct.ThrowIfCancellationRequested();
        var value = await task.ConfigureAwait(false);
        return await mapper(value, ct).ConfigureAwait(false);
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
}
