using System;
using System.Collections.Generic;
using System.Text;

namespace ResultSharp
{
	public readonly partial struct Result
	{
		public static Result<T, E> Ok<T, E>(T val) => val;
		public static Result<T, E> Err<T, E>(E err) => err;
		public static Result<T> Ok<T>(T val) => val;
		public static Result<T> Err<T>(string err) => err;
		public static Result<T, E> Try<T, E>(Func<T> fn) where E : Exception
		{
			try
			{
				var val = fn();
				return val;
			}
			catch (E exception)
			{
				return exception;
			}
		}
		public static Result<T, Exception> Try<T>(Func<T> fn) =>
			Try<T, Exception>(fn);
		public static Result<T, E> OkIf<T, E>(bool condition, T val, E err) =>
			condition switch
			{
				true => val,
				false => err,
			};
		public static Result<T, E> ErrIf<T, E>(bool condition, T val, E err) =>
			OkIf(!condition, val, err);
		public static Result<T> OkIf<T>(bool condition, T val, string err) =>
			OkIf<T, string>(condition, val, err);
		public static Result<T> ErrIf<T>(bool condition, T val, string err) =>
			ErrIf<T, string>(condition, val, err);
		public static Result OkIf(bool condition, string err) =>
			OkIf(condition, Unit.Default, err);
		public static Result ErrIf(bool condition, string err) =>
			OkIf(!condition, err);
	}
}
