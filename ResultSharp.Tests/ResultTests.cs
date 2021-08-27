using System;
using Xunit;
using FluentAssertions;
using FluentAssertions.ResultSharp;
using static ResultSharp.Prelude;

namespace ResultSharp.Tests
{
	public class ResultTests
	{
		[Fact]
		public void Ok_IsOk_ReturnsTrue()
		{
			var result = Result.Ok(0);

			result.IsOk.Should().BeTrue();
		}

		[Fact]
		public void Ok_IsErr_ReturnsFalse()
		{
			var result = Result.Ok(0);

			result.IsErr.Should().BeFalse();
		}

		[Fact]
		public void Err_IsOk_ReturnsFalse()
		{
			var result = Result.Err<string, int>(0);

			result.IsOk.Should().BeFalse();
		}

		[Fact]
		public void Err_IsErr_ReturnsTrue()
		{
			var result = Result.Err<string, int>(0);

			result.IsErr.Should().BeTrue();
		}

		[Fact]
		public void Unwrap_OkResult_ShouldNotThrow()
		{
			Result<int, string> result = Ok(0);

			Func<int> unwrap = result.Unwrap;

			unwrap.Should().NotThrow("because the result is ok");
		}

		[Fact]
		public void UnwrapOr_OkResult_ShouldReturnValue()
		{
			Result<int, string> result = Ok(0);

			var actual = result.UnwrapOr(-1);

			actual.Should().Be(0);
		}

		[Fact]
		public void UnwrapOr_FaultedResult_ShouldReturnOtherValue()
		{
			Result<int, string> result = Err("err");

			var actual = result.UnwrapOr(0);

			actual.Should().Be(0);
		}

		[Fact]
		public void UnwrapOrElse_OkResult_ShouldReturnOkValue()
		{
			Result<int, string> result = Ok(0);

			var actual = result.UnwrapOrElse(() => -1);

			actual.Should().Be(0);
		}

		[Fact]
		public void UnwrapOrElse_FaultedResult_ShouldReturnValueFromClosure()
		{
			Result<int, string> result = Err("err");

			var actual = result.UnwrapOrElse(() => 0);

			actual.Should().Be(0);
		}

		[Fact]
		public void UnwrapOrElse_FaultedResult_ShouldReturnValueFromClosureBasedOnError()
		{
			Result<string, string> result = Err("err");

			var actual = result.UnwrapOrElse(err => $"computed {err}");

			actual.Should().Be("computed err");
		}

		[Fact]
		public void UnwrapErr_OkResult_ShouldThrow()
		{
			Result<int, string> result = Ok(0);

			Func<string> unwrapErr = result.UnwrapErr;

			unwrapErr.Should().Throw<UnwrapErrException>("because the result is ok");
		}

		[Fact]
		public void Unwrap_FaultedResult_ShouldThrow()
		{
			Result<string, int> result = Err(0);

			Func<string> unwrap = result.Unwrap;

			unwrap.Should().Throw<UnwrapException>("because the result is faulted");
		}

		[Fact]
		public void UnwrapErr_FaultedResult_ShouldNotThrow()
		{
			Result<string, int> result = Err(0);

			Func<int> unwrapErr = result.UnwrapErr;

			unwrapErr.Should().NotThrow("because the result is faulted");
		}

		[Fact]
		public void Match_OkResult_ShouldReturnTheValue()
		{
			Result<int, string> result = Ok(10);

			var actual = result.Match(value => value, _ => -1);

			actual.Should().Be(10);
		}

		[Fact]
		public void Match_FaultedResult_ShouldReturnTheError()
		{
			Result<string, int> result = Err(-1);

			var actual = result.Match(_ => 10, err => err);

			actual.Should().Be(-1);
		}

		[Fact]
		public void Map_OkResult_ShouldReturnOkResultWithMappedValue()
		{
			Result<int, string> result = Ok(10);

			var expected = Ok(20);
			var actual = result.Map(val => val * 2);

			actual.Should().Be(expected);
		}

		[Fact]
		public void Map_FaultedResult_ShouldNotMutate()
		{
			Result<string, int> result = Err(0);

			var expected = Err(0);
			var actual = result.Map(str => str.Length);

			actual.Should().Be(expected);
		}

		[Fact]
		public void BiMap_OkResult_ShouldReturnOkMappedResult()
		{
			Result<int, string> result = Ok(10);

			var expected = Ok(20);
			var actual = result.BiMap(val => val * 2, _ => "Unexpected");

			actual.Should().Be(expected);
		}

		[Fact]
		public void BiMap_FaultedResult_ShouldReturnMappedFaultedResult()
		{
			Result<int, string> result = Err("foo");

			var expected = Err("foobar");
			var actual = result.BiMap(val => "Unexpected", err => err + "bar");

			actual.Should().Be(expected);
		}

		[Fact]
		public void MapErr_FaultedResult_ShouldReturnFaultedResultWithMappedError()
		{
			Result<string, int> result = Err(0);

			var expected = Err("code: 0");
			var actual = result.MapErr(err => $"code: {err}");

			actual.Should().Be(expected);
		}

		[Fact]
		public void MapErr_OkResult_ShouldNotMutateResult()
		{
			Result<int, string> result = Ok(10);

			var expected = Ok(10);
			var actual = result.MapErr(_ => -1);

			actual.Should().Be(expected);
		}

		[Fact]
		public void And_OkResult_ShouldReturnOtherResult()
		{
			Result<int, string> a = Ok(10);
			Result<int, string> b = Ok(20);

			var expected = Ok(20);
			var actual = a.And(b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void And_FaultedResult_ShouldNotReturnTheOtherResult()
		{
			Result<string, int> a = Err(-1);
			Result<string, int> b = Ok("ok");

			var expected = Err(-1);
			var actual = a.And(b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void AndThen_OkResult_ShouldReturnAMappedResult()
		{
			Result<int, string> result = Ok(10);

			var expected = Ok(30);
			var actual = result.AndThen<int>(x => Ok(x * 3));

			actual.Should().Be(expected);
		}

		[Fact]
		public void AndThen_FaultedResult_ShouldNotRunTheAndThenClosure()
		{
			Result<int, string> result = Err("err");

			var expected = Err("err");
			var actual = result.AndThen<int>(x => Ok(x * 3));

			actual.Should().Be(expected);
		}

		[Fact]
		public void Or_OkResult_ShouldNotReturnTheOrResult()
		{
			Result<int, string> a = Ok(10);
			Result<int, string> b = Ok(20);

			var expected = Ok(10);
			var actual = a.Or(b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void Or_FaultedResult_ShouldReturnTheOrResult()
		{
			Result<int, string> a = Err("err");
			Result<int, string> b = Ok(20);

			var expected = Ok(20);
			var actual = a.Or(b);

			actual.Should().Be(expected);
		}

		[Fact]
		public void OrElse_OkResult_ShouldNotRunOrClosure()
		{
			Result<int, string> result = Ok(10);

			var expected = Ok(10);
			var actual = result.OrElse<int>(_ => Ok(0));

			actual.Should().Be(expected);
		}

		[Fact]
		public void OrElse_FaultedResult_ShouldReturnOrClosure()
		{
			Result<string, int> result = Err(-1);

			var expected = Ok("-1");
			var actual = result.OrElse<int>(err => Ok(err.ToString()));

			actual.Should().Be(expected);
		}

		[Fact]
		public void Expect_OkResult_ShouldNotThrowAndShouldReturnValue()
		{
			Result<int, string> result = Ok(10);

			Func<int> expect = () => result.Expect("err");

			expect.Should().NotThrow("because the result is ok").Which.Should().Be(10);
		}

		[Fact]
		public void Expect_FaultedResult_ShouldThrow()
		{
			Result<string, int> result = Err(-1);

			Func<string> expect = () => result.Expect("err");

			expect.Should().Throw<ExpectException>("because the result is faulted");
		}

		[Fact]
		public void ExpectErr_OkResult_ShouldThrow()
		{
			Result<int, string> result = Ok(10);

			Func<string> expectErr = () => result.ExpectErr("err");

			expectErr.Should().Throw<ExpectErrException>("because the result is ok");
		}

		[Fact]
		public void ExpectErr_FaultedResult_ShouldNotThrowAndShouldReturnErr()
		{
			Result<string, int> result = Err(-1);

			Func<int> expectErr = () => result.ExpectErr("err");

			expectErr.Should().NotThrow().Which.Should().Be(-1);
		}

		[Fact]
		public void Serialize_And_Deserialize_OkResult()
		{
			Result<string, int> result = Ok("fooBAR");

			var json = SerializationHelpers.Serialize(result);

			var expected = result;
			var actual = SerializationHelpers.Deserialize<Result<string, int>>(json);

			actual.Should().Be(expected);
		}

		[Fact]
		public void Serialize_And_Deserialize_FaultedResult()
		{
			Result<string, int> result = Err(-1);

			var json = SerializationHelpers.Serialize(result);

			var expected = result;
			var actual = SerializationHelpers.Deserialize<Result<string, int>>(json);

			actual.Should().Be(expected);
		}

		[Fact]
		public void GetHashCode_FromOkResult_ShouldReturnHashCodeDifferentFromInnerValue()
		{
			Result<string, int> result = Ok("foobar");

			var valueHashCode = result.Unwrap().GetHashCode();
			var resultHashCode = result.GetHashCode();

			Assert.NotEqual(valueHashCode, resultHashCode);
		}

		[Fact]
		public void GetHashCoide_FromFaultedResult_ShouldReturnHashCodeDifferentFromErrorValue()
		{
			Result<int, string> result = Err("foobar");

			var errHashCode = result.UnwrapErr().GetHashCode();
			var resultHashCode = result.GetHashCode();

			Assert.NotEqual(errHashCode, resultHashCode);
		}

		[Fact]
		public void GetHashCode_FromOkAndErrResult_ShouldReturnDifferentHashCodes()
		{
			Result<string, string> ok = Ok("foobar");
			Result<string, string> err = Err("foobar");

			var okHashCode = ok.GetHashCode();
			var errHashCode = err.GetHashCode();

			Assert.NotEqual(okHashCode, errHashCode);
		}

		[Fact]
		public void ToString_OkResult_ShouldReturnOkStringWithValue()
		{
			Result<string, int> result = Ok("foobar");

			var expected = "Ok(foobar)";
			var actual = result.ToString();

			actual.Should().Be(expected);
		}

		[Fact]
		public void ToString_OkResultOfNullableType_ShouldReturnOkStringWithNull()
		{
			Result<string?, int> result = Ok<string?>(null);

			var expected = "Ok(null)";
			var actual = result.ToString();

			actual.Should().Be(expected);
		}

		[Fact]
		public void Equals_OkResultContainingNull_ShouldReturnTrue()
		{
			Result<string?, int> result = Ok<string?>(null);

			var expected = Result.Ok<string?, int>(null);

			result.Should().Be(expected);
		}

		[Fact]
		public void Equals_FaultedResultContainingNull_ShouldReturnTrue()
		{
			Result<int, int?> result = Err<int?>(null);

			var expected = Result.Err<int, int?>(null);

			result.Should().Be(expected);
		}

		[Fact]
		public void Match_OnOkResultWithoutReturnValues_SetsValue()
		{
			Result<int, string> result = Ok(1);

			int? actual = null;
			var expected = 1;

			_ = result.Match(
				val => actual = val,
				_ => throw new Exception()
			);

			actual.Should().Be(expected);
		}

		[Fact]
		public void Match_OnFaultedREsultWithoutReturnValues_SetsValue()
		{
			Result<double, string> result = Err("foo");

			string? actual = null;
			var expected = "foo";

			result.Match(
				_ => throw new Exception(),
				new Action<string>(err => actual = err)
			);

			actual.Should().Be(expected);
		}
	}
}
