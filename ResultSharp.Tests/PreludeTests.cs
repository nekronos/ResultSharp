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
		public void OkIf_FalseCondition_ReturnsFaultedResult()
		{
			var expected = Err(1);

			var actual = OkIf(false, 0, 1);

			actual.Should().Be(expected);
		}
	}
}
