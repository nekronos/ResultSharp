using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Text;

namespace ResultSharp
{
	[Serializable]
	public readonly struct Result<T> :
		ISerializable,
		IEquatable<Result<T>>,
		IResult
	{
		readonly Result<T, string> Inner;

		Result(Result<T, string> inner) =>
			Inner = inner;

		Result(
			SerializationInfo info,
			StreamingContext context)
		{
			Inner = (Result<T, string>)info.GetValue(nameof(Inner), typeof(Result<T, string>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		public bool IsOk => Inner.IsOk;

		public bool IsErr => Inner.IsErr;

		[Pure]
		public Ret Match<Ret>(Func<T, Ret> ok, Func<string, Ret> err) =>
			Inner.Match(ok, err);

		[Pure]
		public Result<U> Map<U>(Func<T, U> op) =>
			Inner.Map(op);

		[Pure]
		public Result<T, E> MapErr<E>(Func<string, E> op) =>
			Inner.MapErr(op);

		[Pure]
		public Result<U> And<U>(Result<U> result) =>
			Inner.And(result.Inner);

		[Pure]
		public Result<U> AndThen<U>(Func<T, Result<U>> op) =>
			Inner.AndThen<U>(x => op(x));

		[Pure]
		public Result<T> Or(Result<T> result) =>
			Inner.Or(result.Inner);

		[Pure]
		public Result<T> OrElse(Func<string, Result<T>> op) =>
			Inner.OrElse<string>(x => op(x));

		[Pure]
		public T Unwrap() =>
			Inner.Unwrap();

		[Pure]
		public T UnwrapOr(T defaultValue) =>
			Inner.UnwrapOr(defaultValue);

		[Pure]
		public T UnwrapOrElse(Func<T> op) =>
			Inner.UnwrapOrElse(op);

		[Pure]
		public string UnwrapErr() =>
			Inner.UnwrapErr();

		[Pure]
		public T Expect(string msg) =>
			Inner.Expect(msg);

		[Pure]
		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		[Pure]
		public static Result<T> Ok(T value) =>
			new Result<T>(Result<T, string>.Ok(value));

		[Pure]
		public static Result<T> Err(string error) =>
			new Result<T>(Result<T, string>.Err(error));

		[Pure]
		public static implicit operator Result<T>(T value) =>
			Ok(value);

		[Pure]
		public static implicit operator Result<T>(string error) =>
			Err(error);

		[Pure]
		public static implicit operator Result<T>(ResultOk<T> resultOk) =>
			new Result<T>(resultOk);

		[Pure]
		public static implicit operator Result<T>(ResultErr<string> resultErr) =>
			new Result<T>(resultErr);

		[Pure]
		public static implicit operator Result<T, string>(Result<T> result) =>
			result.Inner;

		[Pure]
		public static implicit operator Result<T>(Result<T, string> result) =>
			new Result<T>(result);

		public override string ToString() =>
			Inner.ToString();

		[Pure]
		public override bool Equals(object obj) =>
			obj switch
			{
				Result<T> x => Equals(x),
				_ => Inner.Equals(obj),
			};

		[Pure]
		public override int GetHashCode() =>
			Inner.GetHashCode();

		object IResult.UnwrapErrUntyped() =>
			UnwrapErr();

		public bool Equals(Result<T> other) =>
			Inner.Equals(other.Inner);
	}
}
