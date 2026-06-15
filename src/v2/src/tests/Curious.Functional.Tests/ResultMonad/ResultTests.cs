using Curious.Functional.Monads;
using Curious.Functional.ValueObjects;
using Shouldly;

namespace Curious.Functional.Tests.ResultMonad;

public class ResultTests
{
    private sealed record TestError(string Message) : Error;

    [Fact]
    public void Ok_SetsOkStateAndValue()
    {
        var result = Result<int>.Ok(5);

        result.IsOk.ShouldBeTrue();
        result.IsError.ShouldBeFalse();
        result.TryGetValue(out var value).ShouldBeTrue();
        value.ShouldBe(5);
    }

    [Fact]
    public void Fail_SetsErrorState()
    {
        var error = new TestError("boom");
        var result = Result<int>.Fail(error);

        result.IsOk.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
        result.TryGetValue(out var value).ShouldBeFalse();
        value.ShouldBe(0);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesOk()
    {
        Result<int> result = 42;

        result.IsOk.ShouldBeTrue();
        result.Unwrap().ShouldBe(42);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFail()
    {
        var error = new TestError("boom");
        Result<int> result = error;

        result.IsError.ShouldBeTrue();
        result.ToString().ShouldBe("Fail(TestError { Message = boom })");
    }

    [Fact]
    public void Match_WithResult_OnOk_UsesOkBranch()
    {
        var result = Result<int>.Ok(5);

        var value = result.Match(
            ok: number => number * 2,
            fail: error => error.ToString().Length);

        value.ShouldBe(10);
    }

    [Fact]
    public void Match_WithResult_OnFail_UsesFailBranch()
    {
        var result = Result<int>.Fail(new TestError("boom"));

        var value = result.Match(
            ok: number => number * 2,
            fail: error => error.ToString().Length);

        value.ShouldBe("TestError { Message = boom }".Length);
    }

    [Fact]
    public void Match_WithActions_OnOk_InvokesOkBranchOnly()
    {
        var result = Result<int>.Ok(5);
        var captured = 0;
        var failInvoked = false;

        var matchResult = result.Match(
            ok: number => captured = number,
            fail: _ => failInvoked = true);

        matchResult.ShouldBe(Unit.Default);
        captured.ShouldBe(5);
        failInvoked.ShouldBeFalse();
    }

    [Fact]
    public void Match_WithActions_OnFail_InvokesFailBranchOnly()
    {
        var error = new TestError("boom");
        var result = Result<int>.Fail(error);
        var okInvoked = false;
        var captured = string.Empty;

        result.Match(
            ok: _ => okInvoked = true,
            fail: ex => captured = ex.ToString());

        okInvoked.ShouldBeFalse();
        captured.ShouldBe("TestError { Message = boom }");
    }

    [Fact]
    public void Map_OnOk_TransformsValue()
    {
        var result = Result<int>.Ok(5);

        var mapped = result.Map(value => value.ToString());

        mapped.IsOk.ShouldBeTrue();
        mapped.Unwrap().ShouldBe("5");
    }

    [Fact]
    public void Map_OnFail_ReturnsFailWithoutInvokingMapper()
    {
        var error = new TestError("boom");
        var result = Result<int>.Fail(error);
        var invoked = false;

        var mapped = result.Map(value =>
        {
            invoked = true;
            return value.ToString();
        });

        mapped.IsError.ShouldBeTrue();
        mapped.ToString().ShouldBe("Fail(TestError { Message = boom })");
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void Bind_OnOk_ReturnsBoundResult()
    {
        var result = Result<string>.Ok("42");

        var bound = result.Bind(value => Result<int>.Ok(int.Parse(value)));

        bound.IsOk.ShouldBeTrue();
        bound.Unwrap().ShouldBe(42);
    }

    [Fact]
    public void Bind_OnFail_ReturnsFailWithoutInvokingBinder()
    {
        var error = new TestError("boom");
        var result = Result<string>.Fail(error);
        var invoked = false;

        var bound = result.Bind(value =>
        {
            invoked = true;
            return Result<int>.Ok(value.Length);
        });

        bound.IsError.ShouldBeTrue();
        bound.ToString().ShouldBe("Fail(TestError { Message = boom })");
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void Unwrap_OnFail_Throws()
    {
        var error = new TestError("boom");
        var result = Result<int>.Fail(error);

        var thrown = Should.Throw<InvalidOperationException>(() => result.Unwrap());

        thrown.Message.ShouldBe("Cannot unwrap a failed Result: TestError { Message = boom }");
    }

    [Fact]
    public void UnwrapOr_OnFail_ReturnsFallback()
    {
        var result = Result<int>.Fail(new TestError("boom"));

        result.UnwrapOr(10).ShouldBe(10);
    }

    [Fact]
    public void UnwrapOr_OnOk_ReturnsValue()
    {
        var result = Result<int>.Ok(5);

        result.UnwrapOr(10).ShouldBe(5);
    }

    [Fact]
    public void UnwrapOrElse_FuncWithoutError_OnFail_InvokesFallback()
    {
        var result = Result<int>.Fail(new TestError("boom"));
        var invoked = false;

        var value = result.UnwrapOrElse(() =>
        {
            invoked = true;
            return 10;
        });

        value.ShouldBe(10);
        invoked.ShouldBeTrue();
    }

    [Fact]
    public void UnwrapOrElse_FuncWithoutError_OnOk_DoesNotInvokeFallback()
    {
        var result = Result<int>.Ok(5);
        var invoked = false;

        var value = result.UnwrapOrElse(() =>
        {
            invoked = true;
            return 10;
        });

        value.ShouldBe(5);
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void UnwrapOrElse_FuncWithError_OnFail_InvokesFallback()
    {
        var result = Result<int>.Fail(new TestError("boom"));
        var invoked = false;

        var value = result.UnwrapOrElse(error =>
        {
            invoked = true;
            return error.ToString().Length;
        });

        value.ShouldBe("TestError { Message = boom }".Length);
        invoked.ShouldBeTrue();
    }

    [Fact]
    public void UnwrapOrElse_FuncWithError_OnOk_DoesNotInvokeFallback()
    {
        var result = Result<int>.Ok(5);
        var invoked = false;

        var value = result.UnwrapOrElse(_ =>
        {
            invoked = true;
            return 0;
        });

        value.ShouldBe(5);
        invoked.ShouldBeFalse();
    }

    [Fact]
    public void IfOk_OnOk_InvokesAction()
    {
        var result = Result<int>.Ok(5);
        var captured = 0;

        var actionResult = result.IfOk(value => captured = value);

        actionResult.ShouldBe(Unit.Default);
        captured.ShouldBe(5);
    }

    [Fact]
    public void IfOk_OnFail_DoesNotInvokeAction()
    {
        var result = Result<int>.Fail(new TestError("boom"));
        var invoked = false;

        result.IfOk(_ => invoked = true);

        invoked.ShouldBeFalse();
    }

    [Fact]
    public void IfFail_OnFail_InvokesAction()
    {
        var result = Result<int>.Fail(new TestError("boom"));
        var captured = string.Empty;

        var actionResult = result.IfFail(error => captured = error.ToString());

        actionResult.ShouldBe(Unit.Default);
        captured.ShouldBe("TestError { Message = boom }");
    }

    [Fact]
    public void IfFail_OnOk_DoesNotInvokeAction()
    {
        var result = Result<int>.Ok(5);
        var invoked = false;

        result.IfFail(_ => invoked = true);

        invoked.ShouldBeFalse();
    }

    [Fact]
    public void ToString_OnOk_ShowsWrappedValue()
    {
        var result = Result<int>.Ok(5);

        result.ToString().ShouldBe("Ok(5)");
    }

    [Fact]
    public void ToString_OnFail_ShowsErrorMessage()
    {
        var result = Result<int>.Fail(new TestError("boom"));

        result.ToString().ShouldBe("Fail(TestError { Message = boom })");
    }
}
