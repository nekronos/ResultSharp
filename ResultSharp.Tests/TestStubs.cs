using System;

namespace ResultSharp.Tests
{
	class DivideByZeroException : Exception
	{
		public override bool Equals(object obj) =>
			obj is DivideByZeroException ? true : false;
		public override int GetHashCode() => base.GetHashCode();
	}

	static class TestStubs
	{
		public static int Divide(int dividend, int divisor) =>
			divisor switch
			{
				0 => throw new DivideByZeroException(),
				_ => dividend / divisor,
			};
	}
}
