using System;

namespace ResultSharp
{
	public static class Prelude
	{
		public static ResultOk<T> Ok<T>(T value) where T : notnull =>
			new ResultOk<T>(value);

		public static ResultErr<E> Err<E>(E error) where E : notnull =>
			new ResultErr<E>(error);

		public static Result<T, E> OkIf<T, E>(
			bool condition,
			T value,
			E error)
			where T : notnull
			where E : notnull =>
				Result<T, E>.OkIf(condition, value, error);

		public static Result<T, E> OkIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError)
			where T : notnull
			where E : notnull =>
				Result<T, E>.OkIf(condition, getValue, getError);

		public static Result<T, E> ErrIf<T, E>(
			bool condition,
			T value,
			E error)
			where T : notnull
			where E : notnull =>
				OkIf(!condition, value, error);

		public static Result<T, E> ErrIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError)
			where T : notnull
			where E : notnull =>
				OkIf(!condition, getValue, getError);

		public static Result<T, Exception> Try<T>(Func<T> op) =>
			Try<T, Exception>(op);

		public static Result<T, E> Try<T, E>(Func<T> op)
			where T : notnull
			where E : notnull, Exception
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
	}
}
