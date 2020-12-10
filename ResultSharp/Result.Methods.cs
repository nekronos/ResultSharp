using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace ResultSharp
{
	public readonly partial struct Result
	{
		/// <summary>
		/// Initialize an Ok result from the provided value
		/// </summary>
		/// <param name="value">The ok value</param>
		/// <typeparam name="T">Type of the ok value</typeparam>
		/// <typeparam name="E">Type of the err value</typeparam>
		/// <returns>A Result containing the value in the Ok state</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);

		/// <summary>
		/// Initialize a faulted result from the provided value
		/// </summary>
		/// <param name="error">The value of Err type</param>
		/// <typeparam name="T">Type of the ok value</typeparam>
		/// <typeparam name="E">Type of the err value</typeparam>
		/// <returns>A Result containing the value in the Err state</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> Err<T, E>(E error) => Result<T, E>.Err(error);

		/// <summary>
		/// Initialize an Ok result from the provided value
		/// </summary>
		/// <param name="value">The ok value</param>
		/// <typeparam name="T">Type of the ok value</typeparam>
		/// <returns>A Result containing the value in the Ok state</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

		/// <summary>
		/// Initialize a faulted result from the provided value
		/// </summary>
		/// <param name="error">The value of Err type</param>
		/// <typeparam name="T">Type of the ok value</typeparam>
		/// <returns>A Result containing the value in the Err state</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Err<T>(string error) => Result<T>.Err(error);

		/// <summary>
		/// Run the delegate in a try block catching exception of type E
		/// and return as a result
		/// </summary>
		/// <typeparam name="T">Type of the ok value</typeparam>
		/// <typeparam name="E">Type of the error value, must be an exception</typeparam>
		/// <param name="fn">Delegate to try</param>
		/// <returns>A result representing either the value returned by fn or the exception thrown</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> Try<T, E>(Func<T> fn) where E : Exception
		{
			try
			{
				var value = fn();
				return Result<T, E>.Ok(value);
			}
			catch (E exception)
			{
				return Result<T, E>.Err(exception);
			}
		}

		/// <summary>
		/// Run the delegate in a try block catching all exceptions
		/// </summary>
		/// <typeparam name="T">Type of the ok value</typeparam>
		/// <param name="fn">Delegate to try</param>
		/// <returns>A result representing either the value returned by fn or the exception thrown</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, Exception> Try<T>(Func<T> fn) =>
			Try<T, Exception>(fn);

		/// <summary>
		/// Returns Ok if the condition is true,
		/// otherwise returns Error
		/// </summary>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> OkIf<T, E>(bool condition, T value, E error) =>
			condition
				? Ok<T, E>(value)
				: Err<T, E>(error);

		/// <summary>
		/// Returns Error if the condition is true,
		/// otherwise returns Ok
		/// </summary>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> ErrIf<T, E>(bool condition, T value, E error) =>
			OkIf(!condition, value, error);

		/// <summary>
		/// Returns Ok if the condition is true,
		/// otherwise returns Error
		/// </summary>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> OkIf<T>(bool condition, T value, string error) =>
			OkIf<T, string>(condition, value, error);

		/// <summary>
		/// Returns Error if the condition is true,
		/// otherwise returns Ok
		/// </summary>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> ErrIf<T>(bool condition, T value, string error) =>
			ErrIf<T, string>(condition, value, error);

		/// <summary>
		/// Returns Ok if the condition is true,
		/// otherwise returns Error
		/// </summary>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result OkIf(bool condition, string error) =>
			OkIf(condition, Unit.Default, error);

		/// <summary>
		/// Returns Error if the condition is true,
		/// otherwise returns Ok
		/// </summary>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result ErrIf(bool condition, string error) =>
			OkIf(!condition, error);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> OkIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError) =>
			condition
				? Ok<T, E>(getValue())
				: Err<T, E>(getError());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> ErrIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError) =>
			OkIf(!condition, getValue, getError);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result OkIf(
			bool condition,
			Func<string> getError) =>
			condition ? Ok() : Err(getError());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result ErrIf(
			bool condition,
			Func<string> getError) =>
			OkIf(!condition, getError);
	}
}
