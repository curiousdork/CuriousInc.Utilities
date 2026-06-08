using CuriousInc.Common.Functional.Extensions;
using CuriousInc.Common.Functional.Monads;

namespace CuriousInc.Common.Functional.Tests;

public class MonadAsyncExtensionsTests
{
    private sealed class RequestOtp;

    [Fact]
    public async Task ToAsync_WrapsPlainValueInTask()
    {
        var request = new RequestOtp();

        var result = await request.ToAsync();

        Assert.Same(request, result);
    }

    [Fact]
    public async Task ToAsync_WrapsOptionInTask()
    {
        Option<int> option = 5;

        var result = await option.ToAsync();

        Assert.True(result.TryGetValue(out var value));
        Assert.Equal(5, value);
    }

    [Fact]
    public async Task ToAsync_AdaptsEnumerableToAsyncEnumerable()
    {
        IEnumerable<int> values = new[] { 1, 2, 3 };
        var results = new List<int>();

        await foreach (var item in values.ToAsyncEnumerable(CancellationToken.None))
        {
            results.Add(item);
        }

        Assert.Equal(values, results);
    }

    [Fact]
    public async Task ToAsync_Enumerable_RespectsCancellation()
    {
        IEnumerable<int> values = new[] { 1, 2, 3 };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in values.ToAsyncEnumerable(cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task MapAsync_MapsPlainValue()
    {
        var request = new RequestOtp();

        var result = await request.MapAsync((_, ct) => Task.FromResult("otp"));

        Assert.Equal("otp", result);
    }

    [Fact]
    public async Task MapAsync_MapsAsyncEnumerable()
    {
        IEnumerable<int> values = new[] { 1, 2, 3 };
        var results = new List<int>();

        await foreach (var item in values
                           .ToAsyncEnumerable(TestContext.Current.CancellationToken)
                           .MapAsync((value, ct) => Task.FromResult(value * 2))
                           .WithCancellation(TestContext.Current.CancellationToken))
        {
            results.Add(item);
        }

        Assert.Equal(new[] { 2, 4, 6 }, results);
    }

    [Fact]
    public async Task ApplyAsync_MapsPlainValueWithCancellationToken()
    {
        var request = new RequestOtp();

        var result = await request.ApplyAsync((_, ct) => Task.FromResult("otp"));

        Assert.Equal("otp", result);
    }

    [Fact]
    public async Task OptionMatchAsync_DefaultNullCancellationToken_Works()
    {
        Option<int> option = 5;

        var result = await option.MatchAsync(
            (value, ct) => Task.FromResult(value * 2),
            ct => Task.FromResult(0),
            null);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task OptionMatchAsync_UsesSomeBranch()
    {
        Option<int> option = 5;

        var result = await option.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(0));

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task OptionBindAsync_OnTaskOption_BindsValue()
    {
        Task<Option<int>> optionTask = Task.FromResult<Option<int>>(5);

        var result = await optionTask.BindAsync(value => Task.FromResult(Option<string>.Some($"v:{value}")));

        Assert.True(result.TryGetValue(out var value));
        Assert.Equal("v:5", value);
    }

    [Fact]
    public async Task EitherMapAsync_MapsRightValue()
    {
        Either<string, int> either = 4;

        var result = await either.MapAsync(value => Task.FromResult(value.ToString()));

        Assert.Equal("Right(4)", either.ToString());
        Assert.Equal("Right(4)", Either<string, int>.Right(4).ToString());
        Assert.Equal("Right(4)", result.Match(
            right => $"Right({right})",
            left => $"Left({left})"));
    }

    [Fact]
    public async Task EitherBindAsync_PreservesLeft()
    {
        var either = Either<string, int>.Left("bad");

        var result = await either.BindAsync(value => Task.FromResult(Either<string, string>.Right(value.ToString())));

        Assert.True(result.IsLeft);
        Assert.Equal("bad", result.Unwrapleft());
    }

    [Fact]
    public async Task ResultMapAsync_MapsSuccessfulValue()
    {
        Result<int> result = 7;

        var mapped = await result.MapAsync(value => Task.FromResult(value * 3));

        Assert.True(mapped.TryGetValue(out var value));
        Assert.Equal(21, value);
    }

    [Fact]
    public async Task ResultMapAsync_PassesCancellationTokenToMapper()
    {
        Result<int> result = 7;
        using var cts = new CancellationTokenSource();

        var mapped = await result.MapAsync(
            (value, ct) =>
            {
                Assert.Equal(cts.Token, ct);
                return Task.FromResult(value * 2);
            },
            cts.Token);

        Assert.True(mapped.TryGetValue(out var value));
        Assert.Equal(14, value);
    }

    [Fact]
    public async Task ResultBindAsync_PreservesFailure()
    {
        var error = new Error();
        var result = Result<int>.Fail(error);

        var mapped = await result.BindAsync(value => Task.FromResult(Result<string>.Ok(value.ToString())));

        Assert.True(mapped.IsLeft);
        Assert.False(mapped.TryGetValue(out _));
    }

    [Fact]
    public async Task UnionMatchAsync_DispatchesActiveBranch()
    {
        Union<int, string, bool> union = "hello";

        var result = await union.MatchAsync(
            value => Task.FromResult($"int:{value}"),
            value => Task.FromResult($"string:{value}"),
            value => Task.FromResult($"bool:{value}"));

        Assert.Equal("string:hello", result);
    }

    [Fact]
    public async Task UnionApplyAsync_TransformsSelectedBranchOnly()
    {
        Union<int, string> union = 5;

        var result = await union.ApplyUnionAsync((int value) => Task.FromResult<Union<int, string>>(value + 1));

        Assert.True(result.TryGetValue(out int updated));
        Assert.Equal(6, updated);
    }

    [Fact]
    public async Task UnionMapAsync_TransformsSelectedBranchOnly()
    {
        Union<int, string> union = 5;

        var result = await union.MapAsync((int value) => Task.FromResult<Union<int, string>>(value + 2));

        Assert.True(result.TryGetValue(out int updated));
        Assert.Equal(7, updated);
    }

    [Fact]
    public async Task UnionApplyAsync_PassesCancellationTokenToFunctor()
    {
        Union<int, string> union = 5;
        using var cts = new CancellationTokenSource();

        var result = await union.ApplyUnionAsync(
            (int value, CancellationToken ct) =>
            {
                Assert.Equal(cts.Token, ct);
                return Task.FromResult<Union<int, string>>(value + 1);
            },
            cts.Token);

        Assert.True(result.TryGetValue(out int updated));
        Assert.Equal(6, updated);
    }

    [Fact]
    public async Task MatchAsync_CancelledToken_ThrowsBeforeInvokingBranch()
    {
        Option<int> option = 5;
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            option.MatchAsync(
                (value, ct) => Task.FromResult(value),
                ct => Task.FromResult(0),
                cts.Token));
    }
}
