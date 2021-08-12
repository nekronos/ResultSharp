using System;
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

		Result(SerializationInfo info, StreamingContext context) =>
			Inner = (Result<Unit, string>)info.GetValue(nameof(Inner), typeof(Result<Unit, string>))!;

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result Ok() =>
			new(Ok<Unit, string>(Unit.Default));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result Err(string error) =>
			new(Err<Unit, string>(error));

		/// <inheritdoc cref="Result{T, E}.IsOk" />
		public bool IsOk => Inner.IsOk;

		/// <inheritdoc cref="Result{T, E}.IsErr" />
		public bool IsErr => Inner.IsErr;

		/// <inheritdoc cref="Result{T, E}.OkType" />
		public Type OkType => Inner.OkType;

		/// <inheritdoc cref="Result{T, E}.ErrType" />
		public Type ErrType => Inner.ErrType;

		/// <inheritdoc cref="Result{T, E}.Map{U}(Func{T, U})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> Map<T>(Func<T> op) =>
			Inner.Map(_ => op());

		/// <inheritdoc cref="Result{T, E}.Match{Ret}(Func{T, Ret}, Func{E, Ret})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TRet Match<TRet>(Func<TRet> ok, Func<string, TRet> err) =>
			Inner.Match(_ => ok(), err);

		/// <inheritdoc cref="Result{T, E}.Match(Action{T}, Action{E})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Match(Action ok, Action<string> err) =>
			Inner.Match(_ => ok(), err);

		/// <inheritdoc cref="Result{T, E}.And{U}(Result{U, E})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result And(Result other) =>
			Inner.And(other.Inner);

		/// <inheritdoc cref="Result{T, E}.And{U}(Result{U, E})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> And<T>(Result<T> other) =>
			Inner.And(other.Inner);

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result AndThen(Func<Result> op) =>
			Inner.AndThen<Unit>(_ => op());

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> AndThen<T>(Func<Result<T>> op) =>
			Inner.AndThen<T>(_ => op());

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T> AndThen<T>(Func<Result<T, string>> op) =>
			Inner.AndThen(_ => op());

		/// <inheritdoc cref="Result{T, E}.Or{F}(Result{T, F})" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result Or(Result other) =>
			Inner.Or(other.Inner);

		/// <inheritdoc cref="Result{T, E}.Unwrap" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Unit Unwrap() =>
			Inner.Unwrap();

		/// <inheritdoc cref="Result{T, E}.UnwrapErr" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string UnwrapErr() =>
			Inner.UnwrapErr();

		/// <inheritdoc cref="Result{T, E}.Expect(string)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Unit Expect(string msg) =>
			Inner.Expect(msg);

		/// <inheritdoc cref="Result{T, E}.ExpectErr(string)" />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		public override string ToString() =>
			IsOk ? "Ok()" : Inner.ToString();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Result result) =>
			Inner.Equals(result.Inner);

		public override bool Equals(object? obj) =>
			obj switch
			{
				Result x => Equals(x),
				ResultOk<Unit> x => Equals(x),
				ResultErr<string> x => Equals(x),
				_ => false,
			};

		public override int GetHashCode() =>
			Inner.GetHashCode();

		R IResult.MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err) =>
			((IResult)Inner).MatchUntyped(ok, err);

		object? IResult.UnwrapUntyped() => Unwrap();

		object? IResult.UnwrapErrUntyped() => UnwrapErr();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Result lhs, Result rhs) =>
			lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Result lhs, Result rhs) =>
			lhs.Equals(rhs);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(ResultOk<Unit> _) =>
			Ok();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(ResultErr<string> resultErr) =>
			Err(resultErr.Error);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(Result<Unit, string> result) =>
			new(result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<Unit, string>(Result result) =>
			result.Inner;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result(Result<Unit> result) =>
			result.Inner;
	}
}
