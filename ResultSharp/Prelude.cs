using System;

namespace ResultSharp
{
	public static class Prelude
	{
		public static Result Ok() =>
			Result.Ok();

		public static ResultOk<T> Ok<T>(T value) =>
			new ResultOk<T>(value);

		public static ResultErr<E> Err<E>(E error) =>
			new ResultErr<E>(error);

		public static Result<T, E> OkIf<T, E>(
			bool condition,
			T value,
			E error) =>
				Result<T, E>.OkIf(condition, value, error);

		public static Result<T, E> OkIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError) =>
				Result<T, E>.OkIf(condition, getValue, getError);

		public static Result<T, E> ErrIf<T, E>(
			bool condition,
			T value,
			E error) =>
				OkIf(!condition, value, error);

		public static Result<T, E> ErrIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError) =>
				OkIf(!condition, getValue, getError);

		public static Result<T, Exception> Try<T>(Func<T> op) =>
			Try<T, Exception>(op);

		public static Result<T, E> Try<T, E>(Func<T> op)
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
	}
}
