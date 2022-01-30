using FluentAssertions;
using Xunit;

namespace ResultSharp.Tests;

public class ResultLinqExtensionsTests
{
    [Fact]
    public void SelectMany_ResultTE_Ok()
    {
        var result =
            from a in Result.Ok<string, string>("foo")
            from b in Result.Ok<int, string>(123)
            from c in Result.Ok<string, string>("bar")
            select (a, b, c);

        result.Unwrap().Should().Be(("foo", 123, "bar"));
    }

    [Fact]
    public void SelectMany_ResultTE_Err()
    {
        var result =
            from a in Result.Ok<string, string>("ok")
            from b in Result.Err<string, string>("err1")
            from c in Result.Ok<string, string>("ok")
            from d in Result.Err<string, string>("err2")
            select (a, b, c);

        result.UnwrapErr().Should().Be("err1");
    }

    [Fact]
    public void SelectMany_ResultT_Ok()
    {
        var result =
            from a in Result.Ok("foo")
            from b in Result.Ok(123)
            from c in Result.Ok("bar")
            select (a, b, c);

        result.Unwrap().Should().Be(("foo", 123, "bar"));
    }

    [Fact]
    public void SelectMany_ResultT_Err()
    {
        var result =
            from a in Result.Ok("ok")
            from b in Result.Err<string>("err1")
            from c in Result.Ok("ok")
            from d in Result.Err<string>("err2")
            select (a, b, c);

        result.UnwrapErr().Should().Be("err1");
    }
}