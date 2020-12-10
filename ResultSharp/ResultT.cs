using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ResultSharp
{
	/// <summary>
	/// Union type that can be in one of two states:
	/// Ok(<typeparamref name="T"/>) or Err(string)
	/// </summary>
	/// <typeparam name="T">Bound Ok value</typeparam>
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

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		[Pure]
		public bool IsOk => Inner.IsOk;

		[Pure]
		public bool IsErr => Inner.IsErr;

		[Pure]
		public Type OkType => Inner.OkType;

		[Pure]
		public Type ErrType => Inner.ErrType;

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Result<T> Ok(T value) =>
			new Result<T>(Result<T, string>.Ok(value));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Result<T> Err(string error) =>
			new Result<T>(Result<T, string>.Err(error));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Ret Match<Ret>(Func<T, Ret> ok, Func<string, Ret> err) =>
			Inner.Match(ok, err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Match(Action<T> ok, Action<string> err) =>
			Inner.Match(ok, err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U> Map<U>(Func<T, U> op) =>
			Inner.Map(op);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U> BiMap<U>(Func<T, U> okOp, Func<string, string> errOp) =>
			Inner.BiMap(okOp, errOp);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U, E> BiMap<U, E>(Func<T, U> okOp, Func<string, E> errOp) =>
			Inner.BiMap(okOp, errOp);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> MapErr(Func<string, string> op) =>
			Inner.MapErr(op);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T, E> MapErr<E>(Func<string, E> op) =>
			Inner.MapErr(op);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U> And<U>(Result<U> result) =>
			Inner.And(result.Inner);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U> And<U>(Result<U, string> result) =>
			Inner.And(result);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result AndThen(Func<T, Result> op) =>
			Inner.Match(val => op(val), Result.Err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U> AndThen<U>(Func<T, Result<U>> op) =>
			Inner.AndThen<U>(x => op(x));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U> AndThen<U>(Func<T, Result<U, string>> op) =>
			Inner.AndThen<U>(x => op(x));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> Or(Result<T> result) =>
			Inner.Or(result.Inner);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> OrElse(Func<string, Result<T>> op) =>
			Inner.OrElse<string>(x => op(x));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> OrElse(Func<string, Result<T, string>> op) =>
			Inner.OrElse(x => op(x));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T, E> OrElse<E>(Func<string, Result<T, E>> op) =>
			Inner.OrElse(x => op(x));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Unwrap() =>
			Inner.Unwrap();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T UnwrapOr(T defaultValue) =>
			Inner.UnwrapOr(defaultValue);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T UnwrapOrElse(Func<T> op) =>
			Inner.UnwrapOrElse(op);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string UnwrapErr() =>
			Inner.UnwrapErr();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Expect(string msg) =>
			Inner.Expect(msg);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		[Pure]
		public override string ToString() =>
			Inner.ToString();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Result<T> other) =>
			Inner.Equals(other.Inner);

		[Pure]
		public override bool Equals(object obj) =>
			obj switch
			{
				Result<T> x => Equals(x),
				ResultOk<T> x => Equals(x),
				ResultErr<string> x => Equals(x),
				_ => false,
			};

		[Pure]
		public override int GetHashCode() =>
			Inner.GetHashCode();

		[Pure]
		R IResult.MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err) =>
			((IResult)Inner).MatchUntyped(ok, err);

		[Pure]
		object? IResult.UnwrapUntyped() => Unwrap();

		[Pure]
		object? IResult.UnwrapErrUntyped() => UnwrapErr();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Result<T> lhs, Result<T> rhs) =>
			lhs.Equals(rhs);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Result<T> lhs, Result<T> rhs) =>
			!(lhs == rhs);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T>(ResultOk<T> resultOk) =>
			new Result<T>(resultOk);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static implicit operator Result<T>(ResultErr<string> resultErr) =>
			new Result<T>(resultErr);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T, string>(Result<T> result) =>
			result.Inner;

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T>(Result<T, string> result) =>
			new Result<T>(result);
	}
}
