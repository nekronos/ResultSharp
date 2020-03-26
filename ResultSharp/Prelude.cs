using System;

namespace ResultSharp
{
	public static class Prelude
	{
		public static ResultOk<T> Ok<T>(T value) where T : notnull =>
			new ResultOk<T>(value);

		public static ResultErr<E> Err<E>(E error) where E : notnull =>
			new ResultErr<E>(error);


		public static Result<T, Exception> Try<T>(Func<T> op) =>
			Try<T, Exception>(op);

		public static Result<T,E> Try<T,E>(Func<T> op)
			where E : Exception
		{
			try
			{
				return Ok(op());
			}
			catch (E exception)
			{
				return Err(exception);
			}
		}

		public static Result<T, E> Try<T, E>(Func<T> op, Func<Exception, E> exceptionHandler) =>
			Try<T, Exception>(op)
				.MapErr(exceptionHandler);
	}
}
