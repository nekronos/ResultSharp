using System;
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

		Result(SerializationInfo info, StreamingContext context) =>
			Inner = (Result<T, string>)info.GetValue(nameof(Inner), typeof(Result<T, string>))!;

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		/// <inheritdoc cref="Result{T, E}.IsOk" />
		public bool IsOk => Inner.IsOk;

		/// <inheritdoc cref="Result{T, E}.IsErr" />
		public bool IsErr => Inner.IsErr;

		/// <inheritdoc cref="Result{T, E}.OkType" />
		public Type OkType => Inner.OkType;

		/// <inheritdoc cref="Result{T, E}.ErrType" />
		public Type ErrType => Inner.ErrType;

		internal static Result<T> Ok(T value) =>
			new(Result<T, string>.Ok(value));

		internal static Result<T> Err(string error) =>
			new(Result<T, string>.Err(error));

		/// <inheritdoc cref="Result{T, E}.Match{Ret}(Func{T, Ret}, Func{E, Ret})"/>
		public Ret Match<Ret>(Func<T, Ret> ok, Func<string, Ret> err) =>
			Inner.Match(ok, err);

		/// <inheritdoc cref="Result{T, E}.Match(Action{T}, Action{E})"/>
		public void Match(Action<T> ok, Action<string> err) =>
			Inner.Match(ok, err);

		/// <inheritdoc cref="Result{T, E}.Map{U}(Func{T, U})" />
		public Result<U> Map<U>(Func<T, U> op) =>
			Inner.Map(op);

		/// <inheritdoc cref="Result{T, E}.BiMap{U, F}(Func{T, U}, Func{E, F})" />
		public Result<U> BiMap<U>(Func<T, U> okOp, Func<string, string> errOp) =>
			Inner.BiMap(okOp, errOp);

		/// <inheritdoc cref="Result{T}.BiMap{U}(Func{T, U}, Func{string, string})" />
		public Result<U, E> BiMap<U, E>(Func<T, U> okOp, Func<string, E> errOp) =>
			Inner.BiMap(okOp, errOp);

		/// <inheritdoc cref="Result{T, E}.MapErr{F}(Func{E, F})" />
		public Result<T> MapErr(Func<string, string> op) =>
			Inner.MapErr(op);

		/// <inheritdoc cref="Result{T, E}.MapErr{F}(Func{E, F})" />
		public Result<T, E> MapErr<E>(Func<string, E> op) =>
			Inner.MapErr(op);

		/// <inheritdoc cref="Result{T, E}.And{U}(Result{U, E})" />
		public Result<U> And<U>(Result<U> other) =>
			Inner.And(other.Inner);

		/// <inheritdoc cref="Result{T, E}.And{U}(Result{U, E})" />
		public Result<U> And<U>(Result<U, string> other) =>
			Inner.And(other);

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		public Result AndThen(Func<T, Result> op) =>
			Inner.Match(op, Result.Err);

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		public Result<U> AndThen<U>(Func<T, Result<U>> op) =>
			Inner.AndThen<U>(x => op(x));

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		public Result<U> AndThen<U>(Func<T, Result<U, string>> op) =>
			Inner.AndThen(op);

		/// <inheritdoc cref="Result{T, E}.Or{F}(Result{T, F})" />
		public Result<T> Or(Result<T> other) =>
			Inner.Or(other.Inner);

		/// <inheritdoc cref="Result{T, E}.OrElse{F}(Func{E, Result{T, F}})" />
		public Result<T> OrElse(Func<string, Result<T>> op) =>
			Inner.OrElse<string>(x => op(x));

		/// <inheritdoc cref="Result{T, E}.OrElse{F}(Func{E, Result{T, F}})" />
		public Result<T> OrElse(Func<string, Result<T, string>> op) =>
			Inner.OrElse(op);

		/// <inheritdoc cref="Result{T, E}.OrElse{F}(Func{E, Result{T, F}})" />
		public Result<T, E> OrElse<E>(Func<string, Result<T, E>> op) =>
			Inner.OrElse(op);

		/// <inheritdoc cref="Result{T, E}.Unwrap" />
		public T Unwrap() =>
			Inner.Unwrap();

		/// <inheritdoc cref="Result{T, E}.UnwrapOr(T)" />
		public T UnwrapOr(T defaultValue) =>
			Inner.UnwrapOr(defaultValue);

		/// <inheritdoc cref="Result{T, E}.UnwrapOrElse(Func{T})" />
		public T UnwrapOrElse(Func<T> op) =>
			Inner.UnwrapOrElse(op);

		/// <inheritdoc cref="Result{T, E}.UnwrapOrElse(Func{T})" />
		public T UnwrapOrElse(Func<string, T> op) =>
			Inner.UnwrapOrElse(op);

		/// <inheritdoc cref="Result{T, E}.UnwrapErr" />
		public string UnwrapErr() =>
			Inner.UnwrapErr();

		/// <inheritdoc cref="Result{T, E}.Expect(string)" />
		public T Expect(string msg) =>
			Inner.Expect(msg);

		/// <inheritdoc cref="Result{T, E}.ExpectErr(string)" />
		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		public override string ToString() =>
			Inner.ToString();

		public bool Equals(Result<T> other) =>
			Inner.Equals(other.Inner);

		public override bool Equals(object? obj) =>
			obj switch
			{
				Result<T> x => Equals(x),
				ResultOk<T> x => Equals(x),
				ResultErr<string> x => Equals(x),
				_ => false,
			};

		public override int GetHashCode() =>
			Inner.GetHashCode();

		R IResult.MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err) =>
			((IResult)Inner).MatchUntyped(ok, err);

		object? IResult.UnwrapUntyped() => Unwrap();

		object? IResult.UnwrapErrUntyped() => UnwrapErr();

		public static bool operator ==(Result<T> lhs, Result<T> rhs) =>
			lhs.Equals(rhs);

		public static bool operator !=(Result<T> lhs, Result<T> rhs) =>
			!(lhs == rhs);

		public static implicit operator Result<T>(ResultOk<T> resultOk) =>
			new(resultOk);

		public static implicit operator Result<T>(ResultErr<string> resultErr) =>
			new(resultErr);

		public static implicit operator Result<T, string>(Result<T> result) =>
			result.Inner;

		public static implicit operator Result<T>(Result<T, string> result) =>
			new(result);
	}
}
