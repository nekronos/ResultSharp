using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ResultSharp
{
	internal enum ResultState
	{
		Ok = 1,
		Err,
	}

	/// <summary>
	/// Union type that can be in one of two states:
	/// Ok(<typeparamref name="T"/>) or Err(<typeparamref name="E"/>)
	/// </summary>
	/// <typeparam name="T">Bound Ok value</typeparam>
	/// <typeparam name="E">Bound Err value</typeparam>
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

		void ISerializable.GetObjectData(
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

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Result<T, E> Ok(T value) =>
			new Result<T, E>(value);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Result<T, E> Err(E error) =>
			new Result<T, E>(error);

		/// <summary>
		/// Is the Result in the Ok state
		/// </summary>
		[Pure]
		public bool IsOk =>
			State == ResultState.Ok;

		/// <summary>
		/// Is the Result in the Err state
		/// </summary>
		[Pure]
		public bool IsErr =>
			State == ResultState.Err;

		/// <summary>
		/// Get the Ok type
		/// </summary>
		[Pure]
		public Type OkType =>
			typeof(T);

		/// <summary>
		/// Get the Err type
		/// </summary>
		[Pure]
		public Type ErrType =>
			typeof(E);

		/// <summary>
		/// Match the two states of the Result
		/// </summary>
		/// <typeparam name="Ret">Type of the return value</typeparam>
		/// <param name="ok">Ok match operation</param>
		/// <param name="err">Error match operation</param>
		/// <returns>Ret</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Ret Match<Ret>(Func<T, Ret> ok, Func<E, Ret> err) =>
			IsOk
				? ok(Value)
				: err(Error);

		/// <summary>
		/// Match the two states of the Result
		/// </summary>
		/// <param name="ok">Ok match operation</param>
		/// <param name="err">Error match operation</param>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Match(Action<T> ok, Action<E> err)
		{
			if (IsOk)
				ok(Value);
			else
				err(Error);
		}

		/// <summary>
		/// Project the Ok state from one value to another
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <param name="op">Projection function</param>
		/// <returns>Mapped Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U, E> Map<U>(Func<T, U> op) =>
			BiMap(val => op(val), err => err);


		/// <summary>
		/// Project the Err state from one value to another
		/// </summary>
		/// <typeparam name="F">Resulting Error value type</typeparam>
		/// <param name="op">Projection function</param>
		/// <returns>Mapped Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T, F> MapErr<F>(Func<E, F> op) =>
			BiMap(val => val, err => op(err));

		/// <summary>
		/// Project the Ok or the Err state from one value to another
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <typeparam name="F">Resulting Error value type</typeparam>
		/// <param name="okOp">Ok projection function</param>
		/// <param name="errOp">Err projection function</param>
		/// <returns>Mapped Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U, F> BiMap<U, F>(Func<T, U> okOp, Func<E, F> errOp) =>
			Match(
				val => Result.Ok<U, F>(okOp(val)),
				err => Result.Err<U, F>(errOp(err))
			);

		/// <summary>
		/// Returns other if the result is Ok, otherwise returns the Err value of this.
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <param name="other">the other Result</param>
		/// <returns>Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U, E> And<U>(Result<U, E> other) =>
			Match(_ => other, Result.Err<U, E>);

		/// <summary>
		/// Calls op if the result is Ok, otherwise returns the Err value of this.
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <param name="op">op</param>
		/// <returns>Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<U, E> AndThen<U>(Func<T, Result<U, E>> op) =>
			Match(val => op(val), Result.Err<U, E>);

		/// <summary>
		/// Returns other if the result is Err, otherwise returns the Ok value of this.
		/// </summary>
		/// <typeparam name="F">Resulting Err value type</typeparam>
		/// <param name="other">the other Result</param>
		/// <returns>Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T, F> Or<F>(Result<T, F> other) =>
			Match(Result.Ok<T, F>, _ => other);

		/// <summary>
		/// Calls op if the result is Err, otherwise returns the Ok value of this.
		/// </summary>
		/// <typeparam name="F">Resulting Err value type</typeparam>
		/// <param name="op">op</param>
		/// <returns>Result</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Result<T, F> OrElse<F>(Func<E, Result<T, F>> op) =>
			Match(Result.Ok<T, F>, err => op(err));

		/// <summary>
		/// Returns the contained Ok value, or throws UnwrapException if
		/// the Result is faulted
		/// </summary>
		/// <exception cref="UnwrapException">Thrown if the Result is faulted</exception>
		/// <returns>The contained value</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Unwrap() =>
			Match(
				val => val,
				err => throw new UnwrapException($"{Messages.UnwrapCalledOnAFaultedResult}: '{ToStringNullSafe(err)}'")
			);

		/// <summary>
		/// Returns the contained Ok value, or the provided default value
		/// </summary>
		/// <param name="defaultValue">default value</param>
		/// <returns>The contained or default value</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T UnwrapOr(T defaultValue) =>
			Match(val => val, _ => defaultValue);

		/// <summary>
		/// Returns the contained Ok value, or computes if from the delegate provided
		/// </summary>
		/// <param name="op">operation</param>
		/// <returns>The contained or computed value</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T UnwrapOrElse(Func<T> op) =>
			Match(val => val, _ => op());

		/// <summary>
		/// Returns the contained Err value, or throws UnwrapErrException if
		/// the Result is Ok
		/// </summary>
		/// <exception cref="UnwrapErrException">Thrown if the Result is Ok</exception>
		/// <returns>The contained error</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public E UnwrapErr() =>
			Match(
				val => throw new UnwrapErrException($"{Messages.UnwrapErrCalledOnAnOkResult}: '{ToStringNullSafe(val)}'"),
				err => err
			);

		/// <summary>
		/// Returns the contained Ok value, or throws ExpectException if
		/// the Result is faulted
		/// </summary>
		/// <exception cref="ExpectException">Thrown if the Result is faulted</exception>
		/// <param name="msg">error message</param>
		/// <returns>The contained value</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Expect(string msg) =>
			Match(
				val => val,
				_ => throw new ExpectException(msg)
			);

		/// <summary>
		/// Returns the contained Err value, or throws ExpectErrException if
		/// the Result is Ok
		/// </summary>
		/// <exception cref="ExpectErrException">Thrown if the Result is Ok</exception>
		/// <param name="msg">error message</param>
		/// <returns>The contained error</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public E ExpectErr(string msg) =>
			Match(
				_ => throw new ExpectErrException(msg),
				err => err
			);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string ToStringNullSafe<U>(U val) =>
			val?.ToString() ?? "null";

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() =>
			Match(
				val => $"Ok({ToStringNullSafe(val)})",
				err => $"Err({ToStringNullSafe(err)})"
			);

		/// <summary>
		/// Performs Equality check on the values contained in the results.
		/// If the Results are in different states (eg. Ok and Err) the Equality check
		/// naturally always fails.
		/// </summary>
		/// <param name="other">The other `Result` to compare with this</param>
		/// <returns>True if `this` and `other` are equal</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Result<T, E> other) =>
			other.Match(Equals, Equals);

		[Pure]
		static bool EqualsNullSafe<U>(U a, U b) =>
			a?.Equals(b) ?? b == null;

		bool Equals(T other) =>
			Match(
				val => EqualsNullSafe(val, other),
				_ => false
			);

		[Pure]
		bool Equals(E other) =>
			Match(
				_ => false,
				err => EqualsNullSafe(err, other)
			);

		[Pure]
		public override bool Equals(object obj) =>
			obj switch
			{
				Result<T, E> x => Equals(x),
				ResultOk<T> x => Equals(x),
				ResultErr<E> x => Equals(x),
				_ => false,
			};

		/// <summary>
		/// Calculate the combined hash-code from the contained value and the result state.
		/// Meaning a result of Ok("foo") and Err("foo") does not return the same hashcode
		/// </summary>
		/// <returns>The combined hashcode of the contained value and result state</returns>
		[Pure]
		public override int GetHashCode() =>
			Match(
				val => HashCode.Combine(ResultState.Ok, val),
				err => HashCode.Combine(ResultState.Err, err)
			);

		[Pure]
		R IResult.MatchUntyped<R>(Func<object?, R> okFn, Func<object?, R> errFn) =>
			Match(val => okFn(val), err => errFn(err));

		[Pure]
		object? IResult.UnwrapUntyped => Unwrap();

		[Pure]
		object? IResult.UnwrapErrUntyped => UnwrapErr();

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Result<T, E> lhs, Result<T, E> rhs) =>
			lhs.Equals(rhs);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Result<T, E> lhs, Result<T, E> rhs) =>
			!(lhs == rhs);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T, E>(ResultOk<T> ok) =>
			Ok(ok.Value);

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T, E>(ResultErr<E> err) =>
			Err(err.Error);
	}
}
