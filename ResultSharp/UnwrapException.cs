using System;

namespace ResultSharp
{
	public sealed class UnwrapException : Exception
	{
		internal UnwrapException(string message) : base(message) { }
	}
}
