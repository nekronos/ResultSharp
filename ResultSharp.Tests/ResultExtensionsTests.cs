using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using static ResultSharp.Prelude;

namespace ResultSharp.Tests
{
	public class ResultExtensionsTests
	{
		[Fact]
		public void Combine_ArrayOfResults_ReturnsResultOfIEnumerableContainingErrors()
		{
			IEnumerable<Result<string, int>> results = new Result<string, int>[]
			{
				Ok("foo"),
				Err(1),
				Ok("bar"),
				Err(2),
				Ok("baz"),
				Err(3)
			};

			var expected = new[] { 1, 2, 3 };
			var actual = results.Combine().UnwrapErr();

			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void Combine_ArrayOfResults_ReturnsCombinedResultWithOkValues()
		{
			IEnumerable<Result<string, int>> results = new Result<string, int>[]
			{
				Ok("foo"),
				Ok("bar"),
				Ok("baz"),
			};

			var expected = new[] { "foo", "bar", "baz" };
			var actual = results.Combine().Unwrap();

			actual.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public void AndThenTry_TryChain_ReturnsFaultedResult()
		{
			var expected = Err(new DivideByZeroException());

			var actual = Result<int, DivideByZeroException>
				.Ok(10)
				.AndThenTry(x => TestStubs.Divide(x, 0));

			actual.Should().Be(expected);
		}
	}
}
