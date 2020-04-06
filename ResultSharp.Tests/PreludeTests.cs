using System;
using Xunit;
using FluentAssertions;
using static ResultSharp.Prelude;

namespace ResultSharp.Tests
{
	public class PreludeTests
	{
		class DivideByZeroException : Exception
		{
			public override bool Equals(object obj) =>
				obj is DivideByZeroException ? true : false;
		}

		static int Divide(int dividend, int divisor) =>
			divisor switch
			{
				0 => throw new DivideByZeroException(),
				_ => dividend / divisor,
			};


		[Fact]
		public void Try_CatchesException_ReturnsFaultedResult()
		{
			Result<int, Exception> expected = Err<Exception>(new DivideByZeroException());

			Result<int, Exception> actual = Try(() => Divide(10, 0));

			actual.Should().Be(expected);
		}

		[Fact]
		public void Try_CatchesSpecificException_ReturnsFaultedResult()
		{
			var expected = Err(new DivideByZeroException());

			var actual = Try<int, DivideByZeroException>(() => Divide(10, 0));

			actual.Should().Be(expected);
		}
	}
}
