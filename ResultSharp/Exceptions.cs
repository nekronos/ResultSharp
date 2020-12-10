using System;
using System.Collections.Generic;
using System.Text;

namespace ResultSharp
{
	/// <summary>
	/// Expected the result to be ok
	/// </summary>
	public sealed class ExpectException : Exception
	{
		internal ExpectException(string message) : base(message) { }
	}

	/// <summary>
	/// Expected the result to be faulted
	/// </summary>
	public sealed class ExpectErrException : Exception
	{
		internal ExpectErrException(string message) : base(message) { }
	}

	/// <summary>
	/// Unwrap called on a faulted result
	/// </summary>
	public sealed class UnwrapException : Exception
	{
		internal UnwrapException(string message) : base(message) { }
	}

	/// <summary>
	/// UnwrapErr called on an ok result
	/// </summary>
	public sealed class UnwrapErrException : Exception
	{
		internal UnwrapErrException(string message) : base(message) { }
	}
}
