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
		public void Err_IsOk_ReturnsFalse()
		{
			var result = Result<string, int>.Err(0);

			result.IsOk.Should().BeFalse();
		}

		[Fact]
		public void Unwrap_OkResult_ShouldNotThrow()
		{
			Result<int, string> result = Ok(0);

			Func<int> unwrap = result.Unwrap;

			unwrap.Should().NotThrow("because the result is ok");
		}

		[Fact]
		public void UnwrapErr_OkResult_ShouldThrow()
		{
			Result<int, string> result = Ok(0);

			Func<string> unwrapErr = result.UnwrapErr;

			unwrapErr.Should().Throw<UnwrapException>("because the result is ok");
		}
	}
}
