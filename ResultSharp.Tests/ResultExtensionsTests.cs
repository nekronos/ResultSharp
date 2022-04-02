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

			var actual = Result
				.Ok<int, DivideByZeroException>(10)
				.AndThenTry(x => TestStubs.Divide(x, 0));

			actual.Should().Be(expected);
		}

		[Fact]
		public void Combine_MixedResultsWithoutTypeArgs_ReturnsCombinedErrorMessage()
		{
			IEnumerable<Result> results = new Result[]
			{
				Ok(),
				Err("foo"),
				Ok(),
				Err("bar"),
				Ok(),
				Err("baz")
			};

			var expected = Err("foo\nbar\nbaz");
			var actual = results.Combine("\n");

			actual.Should().Be(expected);
		}

		[Fact]
		public void Combine_OkResultsWithoutTypeArgs_ReturnsOk()
		{
			IEnumerable<Result> results = new Result[]
			{
				Ok(),
				Ok(),
				Ok(),
				Ok(),
			};

			var expected = Ok();
			var actual = results.Combine("\n");

			actual.Should().Be(expected);
		}

		[Fact]
		public void AndThen_ResultTWithResultTE_ReturnsErr()
		{
			Result<int> a = Ok(10);
			Result<int, string> b = Err("foo");

			Result<int> expected = Err("foo");
			var actual = a.AndThen(x => b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void AndThen_ResultTEWithResultT_ReturnsErr()
		{
			Result<int, string> a = Err("foo");
			Result<char> b = Ok('1');

			Result<char> expected = Err("foo");
			var actual = a.AndThen(_ => b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void AndThen_ResultTEWithResult_ReturnsOk()
		{
			Result<int, string> a = Ok(10);
			Result b = Ok();

			Result expected = Ok();
			var actual = a.AndThen(_ => b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void AndThen_ResultWithResultTE_ReturnsErr()
		{
			Result a = Ok();
			Result<int, string> b = Err("foo");

			Result<int> expected = Err("foo");

			var actual = a.AndThen(() => b);
			actual.Should().Be(expected);

			Result<string> x = Ok("asdf");
			Result<int, string> y = Ok(10);

			var z = x.AndThen(_ => y);
		}
	}
}
