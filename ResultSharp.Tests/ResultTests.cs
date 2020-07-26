using System;
using Xunit;
using FluentAssertions;
using static ResultSharp.Prelude;

namespace ResultSharp.Tests
{
	public class ResultTests
	{
		[Fact]
		public void Ok_IsOk_ReturnsTrue()
		{
			var result = Result<int, string>.Ok(0);

			result.IsOk.Should().BeTrue();
		}

		[Fact]
		public void Ok_IsErr_ReturnsFalse()
		{
			var result = Result<int, string>.Ok(0);

			result.IsErr.Should().BeFalse();
		}

		[Fact]
		public void Err_IsOk_ReturnsFalse()
		{
			var result = Result<string, int>.Err(0);

			result.IsOk.Should().BeFalse();
		}

		[Fact]
		public void Err_IsErr_ReturnsTrue()
		{
			var result = Result<string, int>.Err(0);

			result.IsErr.Should().BeTrue();
		}

		[Fact]
		public void OkIf_ConditionIsTrue_ReturnsOkResult()
		{
			var expected = Ok(1);

			var actual = Result<int, string>.OkIf(true, 1, string.Empty);

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_ConditionIsFalse_ReturnsFaultedResult()
		{
			var expected = Err(-1);

			var actual = Result<string, int>.OkIf(false, string.Empty, -1);

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_ConditionIsTrue_ReturnsOkValueFromDelegate()
		{
			var expected = Ok("ok");

			var actual = Result<string, int>.OkIf(
				true,
				() => "ok",
				() => throw new Exception());

			actual.Should().Be(expected);
		}

		[Fact]
		public void OkIf_ConditionIsFalse_ReturnsErrFromDelegate()
		{
			var expected = Err(-1);

			var actual = Result<string, int>.OkIf(
				false,
				() => throw new Exception(),
				() => -1);

			actual.Should().Be(expected);
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
		public void UnwrapErr_OkResult_ShouldThrow()
		{
			Result<int, string> result = Ok(0);

			Func<string> unwrapErr = result.UnwrapErr;

			unwrapErr.Should().Throw<UnwrapException>("because the result is ok");
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
		public void MapOr_OkResult_ShouldReturnMappedValue()
		{
			Result<int, string> result = Ok(10);

			var actual = result.MapOr(x => x * 3, -1);

			actual.Should().Be(30);
		}

		[Fact]
		public void MapOr_FaultedResult_ShouldReturnDefaultValue()
		{
			Result<string, int> result = Err(0);

			var actual = result.MapOr(x => x.Length, 10);

			actual.Should().Be(10);
		}

		[Fact]
		public void MapOrElse_OkResult_ShouldReturnMappedValue()
		{
			Result<int, string> result = Ok(10);

			var actual = result.MapOrElse(x => x * 3, _ => -1);

			actual.Should().Be(30);
		}

		[Fact]
		public void MapOrElse_FaultedResult_ShouldReturnValueFromElseClosure()
		{
			Result<string, int> result = Err(1);

			var actual = result.MapOrElse(_ => -1, err => err);

			actual.Should().Be(1);
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

			expectErr.Should().Throw<ExpectException>("because the result is ok");
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

			var bin = SerializationHelpers.Serialize(result);

			var expected = result;
			var actual = SerializationHelpers.Deserialize<Result<string, int>>(bin);

			actual.Should().Be(expected);
		}

		[Fact]
		public void Serialize_And_Deserialize_FaultedResult()
		{
			Result<string, int> result = Err(-1);

			var bin = SerializationHelpers.Serialize(result);

			var expected = result;
			var actual = SerializationHelpers.Deserialize<Result<string, int>>(bin);

			actual.Should().Be(expected);
		}
	}
}
