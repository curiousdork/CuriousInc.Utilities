using System.Runtime.CompilerServices;

namespace CuriousInc.Common.Functional.Extensions;

/// <summary>
/// Converts synchronous values and sequences into async-friendly shapes.
/// </summary>
public static class ToAsyncExtensions
{
    /// <summary>
    /// Wraps value in completed <see cref="Task{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="value">Value to wrap.</param>
    /// <returns>Completed task containing <paramref name="value"/>.</returns>
    public static Task<T> ToAsync<T>(this T value) =>
        Task.FromResult(value);

    /// <summary>
    /// Adapts synchronous sequence into <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">Sequence item type.</typeparam>
    /// <param name="source">Source sequence.</param>
    /// <param name="cancellationToken">Optional token used while iterating source.</param>
    /// <returns>Async sequence that yields items from <paramref name="source"/>.</returns>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
        this IEnumerable<T> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
            await Task.Yield();
        }
    }
}
