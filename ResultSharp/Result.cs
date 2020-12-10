using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ResultSharp
{
	/// <summary>
	/// Union type that can be in one of two states:
	/// Ok() or Err(string)
	/// </summary>
	[Serializable]
	public readonly partial struct Result :
		ISerializable,
		IEquatable<Result>,
		IResult
	{
		internal readonly Result<Unit, string> Inner;

		Result(Result<Unit, string> inner) =>
			Inner = inner;

		Result(
			SerializationInfo info,
			StreamingContext context)
		{
			Inner = (Result<Unit, string>)info.GetValue(nameof(Inner), typeof(Result<Unit, string>));
		}

		void ISerializable.GetObjectData(
			SerializationInfo info,
			StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result Ok() =>
			new Result(Ok<Unit, string>(Unit.Default));

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result Err(string error) =>
			new Result(Err<Unit, string>(error));

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
		public Result<T> Map<T>(Func<T> op) =>
			Inner.Map(_ => op());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TRet Match<TRet>(Func<TRet> ok, Func<string, TRet> err) =>
			Inner.Match(_ => ok(), err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Match(Action ok, Action<string> err) =>
			Inner.Match(_ => ok(), err);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result And(Result result) =>
			Inner.And(result.Inner);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> And<T>(Result<T> result) =>
			Inner.And(result.Inner);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result AndThen(Func<Result> op) =>
			Inner.AndThen<Unit>(_ => op());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> AndThen<T>(Func<Result<T>> op) =>
			Inner.AndThen<T>(_ => op());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> AndThen<T>(Func<Result<T, string>> op) =>
			Inner.AndThen(_ => op());

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result Or(Result result) =>
			Inner.Or(result.Inner);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Unit Unwrap() =>
			Inner.Unwrap();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string UnwrapErr() =>
			Inner.UnwrapErr();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Expect(string msg) =>
			Inner.Expect(msg);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		[Pure]
		public override string ToString() =>
			Inner.Match(_ => "Ok()", err => $"Err({err})");

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Result result) =>
			Inner.Equals(result.Inner);

		[Pure]
		public override bool Equals(object obj) =>
			obj switch
			{
				Result x => Equals(x),
				ResultOk<Unit> x => Equals(x),
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
		public static bool operator ==(Result lhs, Result rhs) =>
			lhs.Equals(rhs);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Result lhs, Result rhs) =>
			lhs.Equals(rhs);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(ResultOk<Unit> _) =>
			Ok();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(ResultErr<string> resultErr) =>
			Err(resultErr.Error);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(Result<Unit, string> result) =>
			new Result(result);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<Unit, string>(Result result) =>
			result.Inner;

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(Result<Unit> result) =>
			result.Inner;
	}
}
