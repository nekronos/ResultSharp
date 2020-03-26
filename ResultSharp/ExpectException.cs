using System;

namespace ResultSharp
{
	public sealed class ExpectException : Exception
	{
		internal ExpectException(string message) : base(message) { }
	}
}
