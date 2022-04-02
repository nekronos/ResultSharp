using System;

namespace ResultSharp
{
	public static class Prelude
	{
		public static ResultOk<Unit> Ok() =>
			new(Unit.Default);

		public static ResultOk<T> Ok<T>(T value) =>
			new(value);

		public static ResultErr<E> Err<E>(E error) =>
			new(error);

		/// <inheritdoc cref="Result.OkIf{T}(bool, T, string)"/>
		public static Result<T> OkIf<T>(bool condition, T Value, string error) =>
			Result.OkIf(condition, Value, error);

		/// <inheritdoc cref="Result.OkIf{T, E}(bool, T, E)"/>
		public static Result<T, E> OkIf<T, E>(bool condition, T value, E error) =>
			Result.OkIf(condition, value, error);

		/// <inheritdoc cref="Result.OkIf{T, E}(bool, Func{T}, Func{E})"/>
		public static Result<T> OkIf<T>(bool condition, Func<T> getValue, Func<string> getError) =>
			Result.OkIf(condition, getValue, getError);

		/// <inheritdoc cref="Result.OkIf{T, E}(bool, Func{T}, Func{E})"/>
		public static Result<T, E> OkIf<T, E>(bool condition, Func<T> getValue, Func<E> getError) =>
			Result.OkIf(condition, getValue, getError);

		/// <inheritdoc cref="Result.OkIf(bool, string)"/>
		public static Result OkIf(bool condition, string error) =>
			OkIf(condition, Unit.Default, error);

		/// <inheritdoc cref="Result.OkIf(bool, Func{string})"/>
		public static Result OkIf(bool condition, Func<string> getError) =>
			OkIf(condition, () => Unit.Default, getError);

		/// <inheritdoc cref="Result.ErrIf{T}(bool, T, string)"/>
		public static Result<T> ErrIf<T>(bool condition, T value, string error) =>
			Result.ErrIf(condition, value, error);

		/// <inheritdoc cref="Result.ErrIf{T, E}(bool, T, E)"/>
		public static Result<T, E> ErrIf<T, E>(bool condition, T value, E error) =>
			Result.ErrIf(condition, value, error);

		/// <inheritdoc cref="Result.ErrIf{T, E}(bool, Func{T}, Func{E})"/>
		public static Result<T, E> ErrIf<T, E>(bool condition, Func<T> getValue, Func<E> getError) =>
			Result.ErrIf(condition, getValue, getError);

		/// <inheritdoc cref="Result.ErrIf{T, E}(bool, Func{T}, Func{E})"/>
		public static Result<T> ErrIf<T>(bool condition, Func<T> getValue, Func<string> getError) =>
			Result.ErrIf(condition, getValue, getError);

		/// <inheritdoc cref="Result.ErrIf(bool, string)"/>
		public static Result ErrIf(bool condition, string error) =>
			Result.ErrIf(condition, error);

		/// <inheritdoc cref="Result.ErrIf(bool, Func{string})"/>
		public static Result ErrIf(bool condition, Func<string> getError) =>
			Result.ErrIf(condition, getError);

		/// <inheritdoc cref="Result.Try{T}(Func{T})"/>
		public static Result<T, Exception> Try<T>(Func<T> fn) =>
			Result.Try(fn);

		/// <inheritdoc cref="Result.Try{T, E}(Func{T})"/>
		public static Result<T, E> Try<T, E>(Func<T> fn) where E : Exception =>
			Result.Try<T, E>(fn);
	}
}
