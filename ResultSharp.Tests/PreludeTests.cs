using System;
using Xunit;
using FluentAssertions;
using static ResultSharp.Prelude;

namespace ResultSharp.Tests
{
	public class PreludeTests
	{
		[Fact]
		public void Try_CatchesException_ReturnsFaultedResult()
		{
			Result<int, Exception> expected = Err<Exception>(new DivideByZeroException());

			Result<int, Exception> actual = Try(() => TestStubs.Divide(10, 0));

			actual.Should().Be(expected);
		}

		[Fact]
		public void Try_CatchesSpecificException_ReturnsFaultedResult()
		{
			var expected = Err(new DivideByZeroException());

			var actual = Try<int, DivideByZeroException>(() => TestStubs.Divide(10, 0));

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_ConditionIsTrue_ReturnsOkResult()
		{
			var expected = Ok(1);

			var actual = OkIf(true, 1, string.Empty);

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_ConditionIsFalse_ReturnsFaultedResult()
		{
			var expected = Err(-1);

			var actual = OkIf(false, string.Empty, -1);

			actual.Should().Be(expected);

		}

		[Fact]
		public void OkIf_ConditionIsTrue_ReturnsOkValueFromDelegate()
		{
			var expected = Ok("ok");

			var actual = OkIf<string, int>(
				true,
				() => "ok",
				() => throw new Exception());

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_ConditionIsFalse_ReturnsErrFromDelegate()
		{
			var expected = Err(-1);

			var actual = OkIf<int, int>(
				false,
				() => throw new Exception(),
				() => -1);

			actual.Should().Be(expected);
		}
	}
}
