using Curious.Functional.Monads;
using Shouldly;

namespace Curious.Functional.Tests.OptionMonad;

public class OptionTests
{
    [Fact]
    public void ImplicitConversion_FromValueType_CreatesSome()
    {
        Option<int> intOption = 5;

        intOption.HasValue.ShouldBeTrue();
        intOption.Unwrap().ShouldBe(5);
    }

    [Fact]
    public void ImplicitConversion_FromReferenceType_CreatesSome()
    {
        Option<string> stringOption = "hello";

        stringOption.HasValue.ShouldBeTrue();
        stringOption.Unwrap().ShouldBe("hello");
    }

    [Fact]
    public void Some_WithNullReference_ReturnsNone()
    {
        var option = Option<string>.Some(null);

        option.HasValue.ShouldBeFalse();
        option.ToString().ShouldBe("None");
    }

    [Fact]
    public void None_HasNoValue()
    {
        var option = Option<int>.None;

        option.HasValue.ShouldBeFalse();
        option.TryGetValue(out var value).ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void TryGetValue_OnSome_ReturnsTrueAndValue()
    {
        var option = Option<string>.Some("value");

        var result = option.TryGetValue(out var value);

        result.ShouldBeTrue();
        value.ShouldBe("value");
    }

    [Fact]
    public void Map_OnSome_TransformsValue()
    {
        var option = Option<string>.Some("hello");

        var mapped = option.Map(value => value.Length);

        mapped.HasValue.ShouldBeTrue();
        mapped.Unwrap().ShouldBe(5);
    }

    [Fact]
    public void Map_OnNone_ReturnsNoneWithoutInvokingMapper()
    {
        var option = Option<string>.None;
        var invoked = false;

        var mapped = option.Map(value =>
        {
            invoked = true;
            return value.Length;
        });

        mapped.HasValue.ShouldBeFalse();
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void Bind_OnSome_ReturnsBoundOption()
    {
        var option = Option<string>.Some("42");

        var bound = option.Bind(value => Option<int>.Some(int.Parse(value)));

        bound.HasValue.ShouldBeTrue();
        bound.Unwrap().ShouldBe(42);
    }

    [Fact]
    public void Bind_OnNone_ReturnsNoneWithoutInvokingBinder()
    {
        var option = Option<string>.None;
        var invoked = false;

        var bound = option.Bind(value =>
        {
            invoked = true;
            return Option<int>.Some(value.Length);
        });

        bound.HasValue.ShouldBeFalse();
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void Match_WithResult_OnSome_UsesSomeBranch()
    {
        var option = Option<int>.Some(5);

        var result = option.Match(
            some: value => value * 2,
            none: () => 0);

        result.ShouldBe(10);
    }

    [Fact]
    public void Match_WithResult_OnNone_UsesNoneBranch()
    {
        var option = Option<int>.None;

        var result = option.Match(
            some: value => value * 2,
            none: () => 0);

        result.ShouldBe(0);
    }

    [Fact]
    public void Match_WithActions_OnSome_InvokesSomeBranchOnly()
    {
        var option = Option<string>.Some("hello");
        var someValue = string.Empty;
        var noneInvoked = false;

        option.Match(
            some: value => someValue = value,
            none: () => noneInvoked = true);

        someValue.ShouldBe("hello");
        noneInvoked.ShouldBeFalse();
    }

    [Fact]
    public void Match_WithActions_OnNone_InvokesNoneBranchOnly()
    {
        var option = Option<string>.None;
        var someInvoked = false;
        var noneInvoked = false;

        option.Match(
            some: _ => someInvoked = true,
            none: () => noneInvoked = true);

        someInvoked.ShouldBeFalse();
        noneInvoked.ShouldBeTrue();
    }

    [Fact]
    public void Unwrap_OnNone_Throws()
    {
        var option = Option<int>.None;

        Should.Throw<InvalidOperationException>(() => option.Unwrap())
            .Message.ShouldBe("Cannot unwrap an empty Option.");
    }

    [Fact]
    public void UnwrapOr_OnNone_ReturnsFallback()
    {
        var option = Option<int>.None;

        option.UnwrapOr(10).ShouldBe(10);
    }

    [Fact]
    public void UnwrapOr_OnSome_ReturnsValue()
    {
        var option = Option<int>.Some(5);

        option.UnwrapOr(10).ShouldBe(5);
    }

    [Fact]
    public void UnwrapOrElse_OnNone_InvokesFallback()
    {
        var option = Option<int>.None;
        var invoked = false;

        var result = option.UnwrapOrElse(() =>
        {
            invoked = true;
            return 10;
        });

        result.ShouldBe(10);
        invoked.ShouldBeTrue();
    }

    [Fact]
    public void UnwrapOrElse_OnSome_DoesNotInvokeFallback()
    {
        var option = Option<int>.Some(5);
        var invoked = false;

        var result = option.UnwrapOrElse(() =>
        {
            invoked = true;
            return 10;
        });

        result.ShouldBe(5);
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void IfSome_OnSome_InvokesAction()
    {
        var option = Option<string>.Some("hello");
        var captured = string.Empty;

        option.IfSome(value => captured = value);

        captured.ShouldBe("hello");
    }

    [Fact]
    public void IfSome_OnNone_DoesNotInvokeAction()
    {
        var option = Option<string>.None;
        var invoked = false;

        option.IfSome(_ => invoked = true);

        invoked.ShouldBeFalse();
    }

    [Fact]
    public void IfNone_OnNone_InvokesAction()
    {
        var option = Option<int>.None;
        var invoked = false;

        option.IfNone(() => invoked = true);

        invoked.ShouldBeTrue();
    }

    [Fact]
    public void IfNone_OnSome_DoesNotInvokeAction()
    {
        var option = Option<int>.Some(5);
        var invoked = false;

        option.IfNone(() => invoked = true);

        invoked.ShouldBeFalse();
    }

    [Fact]
    public void ToString_OnSome_ShowsWrappedValue()
    {
        var option = Option<string>.Some("hello");

        option.ToString().ShouldBe("Some(hello)");
    }
}
