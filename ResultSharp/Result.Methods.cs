using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace ResultSharp
{
	public readonly partial struct Result
	{
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> Ok<T, E>(T val) => Result<T, E>.Ok(val);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> Err<T, E>(E err) => Result<T, E>.Err(err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Ok<T>(T val) => Result<T>.Ok(val);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Err<T>(string err) => Result<T>.Err(err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> Try<T, E>(Func<T> fn) where E : Exception
		{
			try
			{
				var val = fn();
				return Result<T, E>.Ok(val);
			}
			catch (E exception)
			{
				return Result<T, E>.Err(exception);
			}
		}

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, Exception> Try<T>(Func<T> fn) =>
			Try<T, Exception>(fn);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> OkIf<T, E>(bool condition, T val, E err) =>
			condition switch
			{
				true => Result<T,E>.Ok(val),
				false => Result<T, E>.Err(err),
			};

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T, E> ErrIf<T, E>(bool condition, T val, E err) =>
			OkIf(!condition, val, err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> OkIf<T>(bool condition, T val, string err) =>
			OkIf<T, string>(condition, val, err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> ErrIf<T>(bool condition, T val, string err) =>
			ErrIf<T, string>(condition, val, err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result OkIf(bool condition, string err) =>
			OkIf(condition, Unit.Default, err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result ErrIf(bool condition, string err) =>
			OkIf(!condition, err);
	}
}
