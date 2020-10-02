using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace ResultSharp
{
	internal enum ResultState
	{
		Ok = 1,
		Err,
	}

	[Serializable]
	public readonly struct Result<T, E> :
		ISerializable,
		IEquatable<Result<T, E>>,
		IResult
	{
		readonly ResultState State;
		readonly T Value;
		readonly E Error;

		Result(T value)
		{
			State = ResultState.Ok;
			Value = value;
			Error = default!;
		}

		Result(E error)
		{
			State = ResultState.Err;
			Value = default!;
			Error = error;
		}

		Result(
			SerializationInfo info,
			StreamingContext context)
		{
			State = (ResultState)info.GetValue(nameof(State), typeof(ResultState));
			switch (State)
			{
				case ResultState.Ok:
					Value = (T)info.GetValue(nameof(Value), typeof(T));
					Error = default!;
					break;

				case ResultState.Err:
					Value = default!;
					Error = (E)info.GetValue(nameof(Error), typeof(E));
					break;

				default:
					throw new Exception($"Unexpected state: {State}");
			}
		}

		public void GetObjectData(
			SerializationInfo info,
			StreamingContext context)
		{
			info.AddValue(nameof(State), State);
			switch (State)
			{
				case ResultState.Ok:
					info.AddValue(nameof(Value), Value);
					break;

				case ResultState.Err:
					info.AddValue(nameof(Error), Error);
					break;
			}
		}

		public bool IsOk =>
			State == ResultState.Ok;

		public bool IsErr =>
			State == ResultState.Err;

		public Type OkType =>
			typeof(T);

		public Type ErrType =>
			typeof(E);

		public Ret Match<Ret>(Func<T, Ret> ok, Func<E, Ret> err) =>
			IsOk
				? ok(Value)
				: err(Error);

		public void Match(Action<T> ok, Action<E> err)
		{
			if (IsOk)
				ok(Value);
			else
				err(Error);
		}

		public Result<U, E> Map<U>(Func<T, U> op) =>
			BiMap(val => op(val), err => err);

		public Result<T, F> MapErr<F>(Func<E, F> op) =>
			BiMap(val => val, err => op(err));

		public Result<U, F> BiMap<U, F>(Func<T, U> okOp, Func<E, F> errOp) =>
			Match(
				val => Result.Ok<U, F>(okOp(val)),
				err => Result.Err<U, F>(errOp(err))
			);

		public Result<U, E> And<U>(Result<U, E> result) =>
			Match(_ => result, Result.Err<U, E>);

		public Result<U, E> AndThen<U>(Func<T, Result<U, E>> op) =>
			Match(val => op(val), Result.Err<U, E>);

		public Result<T, F> Or<F>(Result<T, F> result) =>
			Match(Result.Ok<T, F>, _ => result);

		public Result<T, F> OrElse<F>(Func<E, Result<T, F>> op) =>
			Match(Result.Ok<T, F>, err => op(err));

		public T Unwrap() =>
			Match(
				val => val,
				_ => throw new UnwrapException(Messages.UnwrapCalledOnAFaultedResult));

		public T UnwrapOr(T defaultValue) =>
			Match(val => val, _ => defaultValue);

		public T UnwrapOrElse(Func<T> op) =>
			Match(val => val, _ => op());

		public E UnwrapErr() =>
			Match(
				_ => throw new UnwrapException(Messages.UnwrapErrCalledOnAnOkResult),
				err => err);

		public T Expect(string msg) =>
			Match(
				val => val,
				_ => throw new ExpectException(msg));

		public E ExpectErr(string msg) =>
			Match(
				_ => throw new ExpectException(msg),
				err => err);

		static string ToStringNullSafe<U>(U val) =>
			val?.ToString() ?? "null";

		public override string ToString() =>
			Match(
				val => $"Ok({ToStringNullSafe(val)})",
				err => $"Err({ToStringNullSafe(err)})");

		public static Result<T, E> Ok(T value) =>
			new Result<T, E>(value);

		public static Result<T, E> Err(E error) =>
			new Result<T, E>(error);

		static bool EqualsNullSafe<U>(U a, U b) =>
			a?.Equals(b) ?? b == null;

		public bool Equals(Result<T, E> other) =>
			other.Match(Equals, Equals);

		public bool Equals(T other) =>
			Match(val => EqualsNullSafe(val, other), _ => false);

		public bool Equals(E other) =>
			Match(_ => false, err => EqualsNullSafe(err, other));

		public override bool Equals(object obj) =>
			obj switch
			{
				Result<T, E> x => Equals(x),
				ResultOk<T> x => Equals(x),
				ResultErr<E> x => Equals(x),
				_ => false,
			};

		public override int GetHashCode() =>
			Match(
				val => HashCode.Combine(ResultState.Ok, val),
				err => HashCode.Combine(ResultState.Err, err));

		public R MatchUntyped<R>(Func<object?, R> okFn, Func<object?, R> errFn) =>
			Match(val => okFn(val), err => errFn(err));

		object? IResult.UnwrapUntyped() =>
			Unwrap();

		object? IResult.UnwrapErrUntyped() =>
			UnwrapErr();

		public static bool operator ==(Result<T, E> a, Result<T, E> b) =>
			a.Equals(b);

		public static bool operator !=(Result<T, E> a, Result<T, E> b) =>
			!(a == b);

		public static implicit operator Result<T, E>(ResultOk<T> ok) =>
			Ok(ok.Value);

		public static implicit operator Result<T, E>(ResultErr<E> err) =>
			Err(err.Error);
	}
}
