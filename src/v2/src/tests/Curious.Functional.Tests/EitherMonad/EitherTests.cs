using Curious.Functional.Monads;
using Shouldly;

namespace Curious.Functional.Tests.EitherMonad;

public class EitherTests
{
    [Fact]
    public void Right_SetsRightStateAndValue()
    {
        var either = Either<string, int>.Right(5);

        either.IsRight.ShouldBeTrue();
        either.IsLeft.ShouldBeFalse();
        either.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(5);
    }

    [Fact]
    public void Left_SetsLeftState()
    {
        var either = Either<string, int>.Left("error");

        either.IsRight.ShouldBeFalse();
        either.IsLeft.ShouldBeTrue();
        either.TryGetValue(out var value).ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void ImplicitConversion_FromRightValue_CreatesRight()
    {
        Either<string, int> either = 42;

        either.IsRight.ShouldBeTrue();
        either.Unwrap().ShouldBe(42);
    }

    [Fact]
    public void ImplicitConversion_FromLeftValue_CreatesLeft()
    {
        Either<string, int> either = "error";

        either.IsLeft.ShouldBeTrue();
        either.UnwrapLeft().ShouldBe("error");
    }

    [Fact]
    public void Match_WithResult_OnRight_UsesRightBranch()
    {
        var either = Either<string, int>.Right(5);

        var result = either.Match(
            right: value => value * 2,
            left: error => error.Length);

        result.ShouldBe(10);
    }

    [Fact]
    public void Match_WithResult_OnLeft_UsesLeftBranch()
    {
        var either = Either<string, int>.Left("boom");

        var result = either.Match(
            right: value => value * 2,
            left: error => error.Length);

        result.ShouldBe(4);
    }

    [Fact]
    public void Match_WithActions_OnRight_InvokesRightBranchOnly()
    {
        var either = Either<string, int>.Right(5);
        var captured = 0;
        var leftInvoked = false;

        var result = either.Match(
            right: value => captured = value,
            left: _ => leftInvoked = true);

        result.ShouldBe(Unit.Default);
        captured.ShouldBe(5);
        leftInvoked.ShouldBeFalse();
    }

    [Fact]
    public void Match_WithActions_OnLeft_InvokesLeftBranchOnly()
    {
        var either = Either<string, int>.Left("boom");
        var rightInvoked = false;
        var captured = string.Empty;

        either.Match(
            right: _ => rightInvoked = true,
            left: error => captured = error);

        rightInvoked.ShouldBeFalse();
        captured.ShouldBe("boom");
    }

    [Fact]
    public void Map_OnRight_TransformsRightValue()
    {
        var either = Either<string, int>.Right(5);

        var mapped = either.Map(value => value.ToString());

        mapped.IsRight.ShouldBeTrue();
        mapped.Unwrap().ShouldBe("5");
    }

    [Fact]
    public void Map_OnLeft_ReturnsLeftWithoutInvokingMapper()
    {
        var either = Either<string, int>.Left("boom");
        var invoked = false;

        var mapped = either.Map(value =>
        {
            invoked = true;
            return value.ToString();
        });

        mapped.IsLeft.ShouldBeTrue();
        mapped.UnwrapLeft().ShouldBe("boom");
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void MapLeft_OnLeft_TransformsLeftValue()
    {
        var either = Either<string, int>.Left("boom");

        var mapped = either.MapLeft(error => error.Length);

        mapped.IsLeft.ShouldBeTrue();
        mapped.UnwrapLeft().ShouldBe(4);
    }

    [Fact]
    public void MapLeft_OnRight_ReturnsRightWithoutInvokingMapper()
    {
        var either = Either<string, int>.Right(5);
        var invoked = false;

        var mapped = either.MapLeft(error =>
        {
            invoked = true;
            return error.Length;
        });

        mapped.IsRight.ShouldBeTrue();
        mapped.Unwrap().ShouldBe(5);
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void Bind_OnRight_ReturnsBoundEither()
    {
        var either = Either<string, string>.Right("42");

        var bound = either.Bind(value => Either<string, int>.Right(int.Parse(value)));

        bound.IsRight.ShouldBeTrue();
        bound.Unwrap().ShouldBe(42);
    }

    [Fact]
    public void Bind_OnLeft_ReturnsLeftWithoutInvokingBinder()
    {
        var either = Either<string, string>.Left("boom");
        var invoked = false;

        var bound = either.Bind(value =>
        {
            invoked = true;
            return Either<string, int>.Right(value.Length);
        });

        bound.IsLeft.ShouldBeTrue();
        bound.UnwrapLeft().ShouldBe("boom");
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void Unwrap_OnLeft_Throws()
    {
        var either = Either<string, int>.Left("boom");

        Should.Throw<InvalidOperationException>(() => either.Unwrap())
            .Message.ShouldBe("Cannot unwrap a Left value: boom");
    }

    [Fact]
    public void UnwrapLeft_OnRight_Throws()
    {
        var either = Either<string, int>.Right(5);

        Should.Throw<InvalidOperationException>(() => either.UnwrapLeft())
            .Message.ShouldBe("Cannot unwrap a Left value from a Right Either.");
    }

    [Fact]
    public void UnwrapOrElse_OnLeft_UsesFallback()
    {
        var either = Either<string, int>.Left("boom");
        var invoked = false;

        var result = either.UnwrapOrElse(error =>
        {
            invoked = true;
            return error.Length;
        });

        result.ShouldBe(4);
        invoked.ShouldBeTrue();
    }

    [Fact]
    public void UnwrapOrElse_OnRight_DoesNotInvokeFallback()
    {
        var either = Either<string, int>.Right(5);
        var invoked = false;

        var result = either.UnwrapOrElse(_ =>
        {
            invoked = true;
            return 0;
        });

        result.ShouldBe(5);
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void IfRight_OnRight_InvokesAction()
    {
        var either = Either<string, int>.Right(5);
        var captured = 0;

        var result = either.IfRight(value => captured = value);

        result.ShouldBe(Unit.Default);
        captured.ShouldBe(5);
    }

    [Fact]
    public void IfRight_OnLeft_DoesNotInvokeAction()
    {
        var either = Either<string, int>.Left("boom");
        var invoked = false;

        either.IfRight(_ => invoked = true);

        invoked.ShouldBeFalse();
    }

    [Fact]
    public void IfLeft_OnLeft_InvokesAction()
    {
        var either = Either<string, int>.Left("boom");
        var captured = string.Empty;

        var result = either.IfLeft(error => captured = error);

        result.ShouldBe(Unit.Default);
        captured.ShouldBe("boom");
    }

    [Fact]
    public void IfLeft_OnRight_DoesNotInvokeAction()
    {
        var either = Either<string, int>.Right(5);
        var invoked = false;

        either.IfLeft(_ => invoked = true);

        invoked.ShouldBeFalse();
    }

    [Fact]
    public void ToString_OnRight_ShowsWrappedValue()
    {
        var either = Either<string, int>.Right(5);

        either.ToString().ShouldBe("Right(5)");
    }

    [Fact]
    public void ToString_OnLeft_ShowsWrappedValue()
    {
        var either = Either<string, int>.Left("boom");

        either.ToString().ShouldBe("Left(boom)");
    }
}
