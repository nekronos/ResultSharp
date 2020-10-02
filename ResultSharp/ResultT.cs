using System;
using System.Runtime.Serialization;

namespace ResultSharp
{
	[Serializable]
	public readonly struct Result<T> :
		ISerializable,
		IEquatable<Result<T>>,
		IResult
	{
		internal readonly Result<T, string> Inner;

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

		public Type OkType => Inner.OkType;

		public Type ErrType => Inner.ErrType;

		public static Result<T> Ok(T value) =>
			Result<T, string>.Ok(value);

		public static Result<T> Err(string error) =>
			Result<T, string>.Err(error);

		public Ret Match<Ret>(Func<T, Ret> ok, Func<string, Ret> err) =>
			Inner.Match(ok, err);

		public void Match(Action<T> ok, Action<string> err) =>
			Inner.Match(ok, err);

		public Result<U> Map<U>(Func<T, U> op) =>
			Inner.Map(op);

		public Result<U> BiMap<U>(Func<T, U> okOp, Func<string, string> errOp) =>
			Inner.BiMap(okOp, errOp);

		public Result<U, E> BiMap<U, E>(Func<T, U> okOp, Func<string, E> errOp) =>
			Inner.BiMap(okOp, errOp);

		public Result<T> MapErr(Func<string, string> op) =>
			Inner.MapErr(op);

		public Result<T, E> MapErr<E>(Func<string, E> op) =>
			Inner.MapErr(op);

		public Result<U> And<U>(Result<U> result) =>
			Inner.And(result.Inner);

		public Result<U> And<U>(Result<U, string> result) =>
			Inner.And(result);

		public Result AndThen(Func<T, Result> op) =>
			Inner.Match(val => op(val), Result.Err);

		public Result<U> AndThen<U>(Func<T, Result<U>> op) =>
			Inner.AndThen<U>(x => op(x));

		public Result<U> AndThen<U>(Func<T, Result<U, string>> op) =>
			Inner.AndThen<U>(x => op(x));

		public Result<T> Or(Result<T> result) =>
			Inner.Or(result.Inner);

		public Result<T> OrElse(Func<string, Result<T>> op) =>
			Inner.OrElse<string>(x => op(x));

		public Result<T> OrElse(Func<string, Result<T, string>> op) =>
			Inner.OrElse(x => op(x));

		public Result<T, E> OrElse<E>(Func<string, Result<T, E>> op) =>
			Inner.OrElse(x => op(x));

		public T Unwrap() =>
			Inner.Unwrap();

		public T UnwrapOr(T defaultValue) =>
			Inner.UnwrapOr(defaultValue);

		public T UnwrapOrElse(Func<T> op) =>
			Inner.UnwrapOrElse(op);

		public string UnwrapErr() =>
			Inner.UnwrapErr();

		public T Expect(string msg) =>
			Inner.Expect(msg);

		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		public override string ToString() =>
			Inner.ToString();

		public bool Equals(Result<T> other) =>
			Inner.Equals(other.Inner);

		public override bool Equals(object obj) =>
			obj switch
			{
				Result<T> x => Equals(x),
				ResultOk<T> x => Equals(x),
				ResultErr<string> x => Equals(x),
				_ => false,
			};

		public override int GetHashCode() =>
			Inner.GetHashCode();

		public R MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err) =>
			Inner.MatchUntyped(ok, err);

		object? IResult.UnwrapUntyped() =>
			Unwrap();

		object? IResult.UnwrapErrUntyped() =>
			UnwrapErr();

		public static bool operator ==(Result<T> a, Result<T> b) =>
			a.Equals(b);

		public static bool operator !=(Result<T> a, Result<T> b) =>
			!(a == b);

		public static implicit operator Result<T>(ResultOk<T> resultOk) =>
			new Result<T>(resultOk);

		public static implicit operator Result<T>(ResultErr<string> resultErr) =>
			new Result<T>(resultErr);

		public static implicit operator Result<T, string>(Result<T> result) =>
			result.Inner;

		public static implicit operator Result<T>(Result<T, string> result) =>
			new Result<T>(result);
	}
}
