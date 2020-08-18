using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;
using static ResultSharp.Prelude;

namespace ResultSharp.Tests
{
	public class ResultMethodsTEsts
	{
		[Fact]
		public void OkIf_TrueCondition_ReturnsOkResult()
		{
			var expected = Ok(10);
			var actual = Result.OkIf(true, 10, "foo");

			actual.Should().Be(expected);
		}

		[Fact]
		public void ErrIf_TrueCondition_ReturnsFaultedResult()
		{
			var expected = Err("foo");
			var actual = Result.ErrIf(true, 0, "foo");

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_TrueConditionWithNonStringErrType_ReturnsOkResult()
		{
			var expected = Ok("foo");
			var actual = Result.OkIf(true, "foo", -1);

			actual.Should().Be(expected);
		}

		[Fact]
		public void ErrIf_TrueConditionWithNonStringErrType_ReturnsFaultedResult()
		{
			var expected = Err(-1);
			var actual = Result.ErrIf(true, "foo", -1);

			actual.Should().Be(expected);
		}
	}
}
