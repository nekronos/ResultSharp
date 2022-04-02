using System;

namespace ResultSharp
{
	public abstract class ResultException : Exception
	{
		protected ResultException(string message) : base(message) { }
	}

	/// <summary>
	/// Expected the result to be ok
	/// </summary>
	public sealed class ExpectException : ResultException
	{
		internal ExpectException(string message) : base(message) { }
	}

	/// <summary>
	/// Expected the result to be faulted
	/// </summary>
	public sealed class ExpectErrException : ResultException
	{
		internal ExpectErrException(string message) : base(message) { }
	}

	/// <summary>
	/// Unwrap called on a faulted result
	/// </summary>
	public sealed class UnwrapException : ResultException
	{
		internal UnwrapException(string message) : base(message) { }
	}

	/// <summary>
	/// UnwrapErr called on an ok result
	/// </summary>
	public sealed class UnwrapErrException : ResultException
	{
		internal UnwrapErrException(string message) : base(message) { }
	}
}
