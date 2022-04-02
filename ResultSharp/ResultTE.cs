using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ResultSharp
{
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
		enum ResultState
		{
			Ok = 1,
			Err,
		}

		readonly ResultState State;
		internal readonly T Value;
		internal readonly E Error;

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

		Result(SerializationInfo info, StreamingContext context)
		{
			State = (ResultState)info.GetValue(nameof(State), typeof(ResultState))!;
			(Value, Error) = State switch
			{
				ResultState.Ok => (
					(T)info.GetValue(nameof(Value), typeof(T))!,
					default(E)!
				),

				ResultState.Err => (
					default(T)!,
					(E)info.GetValue(nameof(Error), typeof(E))!
				),

				_ => throw new Exception($"Unexpected state: {State}")
			};
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(State), State);
			Match(
				ok: value => info.AddValue(nameof(Value), value),
				err: error => info.AddValue(nameof(Error), error)
			);
		}

		internal static Result<T, E> Ok(T value) =>
			new Result<T, E>(value);

		internal static Result<T, E> Err(E error) =>
			new Result<T, E>(error);

		/// <summary>
		/// Is the Result in the Ok state
		/// </summary>
		public bool IsOk =>
			State == ResultState.Ok;

		/// <summary>
		/// Is the Result in the Err state
		/// </summary>
		public bool IsErr =>
			State == ResultState.Err;

		/// <summary>
		/// Get the Ok type
		/// </summary>
		public Type OkType =>
			typeof(T);

		/// <summary>
		/// Get the Err type
		/// </summary>
		public Type ErrType =>
			typeof(E);

		/// <summary>
		/// Match the two states of the Result
		/// </summary>
		/// <typeparam name="Ret">Type of the return value</typeparam>
		/// <param name="ok">Ok match operation</param>
		/// <param name="err">Error match operation</param>
		/// <returns>Ret</returns>
		public Ret Match<Ret>(Func<T, Ret> ok, Func<E, Ret> err) =>
			IsOk
				? ok(Value)
				: err(Error);

		/// <summary>
		/// Match the two states of the Result
		/// </summary>
		/// <param name="ok">Ok match operation</param>
		/// <param name="err">Error match operation</param>
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
		/// <param name="fn">Projection function</param>
		/// <returns>Mapped Result</returns>
		public Result<U, E> Map<U>(Func<T, U> fn) =>
			Match(
				value => Result.Ok<U, E>(fn(value)),
				Result.Err<U, E>
			);

		/// <summary>
		/// Project the Error state from one value to another
		/// </summary>
		/// <typeparam name="F">Resulting Error value type</typeparam>
		/// <param name="fn">Projection function</param>
		/// <returns>Mapped Result</returns>
		public Result<T, F> MapErr<F>(Func<E, F> fn) =>
			Match(
				Result.Ok<T, F>,
				error => Result.Err<T, F>(fn(error))
			);

		/// <summary>
		/// Project the Ok or the Error state from one value to another
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <typeparam name="F">Resulting Error value type</typeparam>
		/// <param name="ok">Ok projection function</param>
		/// <param name="err">Err projection function</param>
		/// <returns>Mapped Result</returns>
		public Result<U, F> BiMap<U, F>(Func<T, U> ok, Func<E, F> err) =>
			Match(
				value => Result.Ok<U, F>(ok(value)),
				error => Result.Err<U, F>(err(error))
			);

		/// <summary>
		/// Returns other if the result is Ok, otherwise returns the Err value of this.
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <param name="other">the other Result</param>
		/// <returns>Result</returns>
		public Result<U, E> And<U>(Result<U, E> other) =>
			Match(_ => other, Result.Err<U, E>);

		/// <summary>
		/// Calls fn if the result is Ok, otherwise returns the Err value of this.
		/// </summary>
		/// <typeparam name="U">Resulting Ok value type</typeparam>
		/// <param name="fn">fn</param>
		/// <returns>Result</returns>
		public Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn) =>
			Match(fn, Result.Err<U, E>);

		/// <summary>
		/// Returns other if the result is Err, otherwise returns the Ok value of this.
		/// </summary>
		/// <typeparam name="F">Resulting Err value type</typeparam>
		/// <param name="other">the other Result</param>
		/// <returns>Result</returns>
		public Result<T, F> Or<F>(Result<T, F> other) =>
			Match(Result.Ok<T, F>, _ => other);

		/// <summary>
		/// Calls fn if the result is Err, otherwise returns the Ok value of this.
		/// </summary>
		/// <typeparam name="F">Resulting Err value type</typeparam>
		/// <param name="fn">fn</param>
		/// <returns>Result</returns>
		public Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn) =>
			Match(Result.Ok<T, F>, fn);

		/// <summary>
		/// Returns the contained Ok value, or throws UnwrapException if
		/// the Result is faulted
		/// </summary>
		/// <exception cref="UnwrapException">Thrown if the Result is faulted</exception>
		/// <returns>The contained value</returns>
		public T Unwrap() =>
			Match(
				value => value,
				error => throw new UnwrapException($"{Messages.UnwrapCalledOnAFaultedResult}: '{ToStringNullSafe(error)}'")
			);

		/// <summary>
		/// Returns the contained Ok value, or the provided default value
		/// </summary>
		/// <param name="defaultValue">default value</param>
		/// <returns>The contained or default value</returns>
		public T UnwrapOr(T defaultValue) =>
			Match(value => value, _ => defaultValue);

		/// <summary>
		/// Returns the contained Ok value, or computes it from the provided delegate
		/// </summary>
		/// <param name="fn">operation</param>
		/// <returns>The contained or computed value</returns>
		public T UnwrapOrElse(Func<T> fn) =>
			Match(value => value, _ => fn());

		/// <summary>
		/// Returns the contained Ok value, or computes it from the error value using the provided delegate
		/// </summary>
		/// <param name="fn">operation</param>
		/// <returns>The contained or computed value</returns>
		public T UnwrapOrElse(Func<E, T> fn) =>
			Match(value => value, fn);

		/// <summary>
		/// Returns the contained Err value, or throws UnwrapErrException if
		/// the Result is Ok
		/// </summary>
		/// <exception cref="UnwrapErrException">Thrown if the Result is Ok</exception>
		/// <returns>The contained error</returns>
		public E UnwrapErr() =>
			Match(
				value => throw new UnwrapErrException($"{Messages.UnwrapErrCalledOnAnOkResult}: '{ToStringNullSafe(value)}'"),
				error => error
			);

		/// <summary>
		/// Returns the contained Ok value, or throws ExpectException if
		/// the Result is faulted
		/// </summary>
		/// <exception cref="ExpectException">Thrown if the Result is faulted</exception>
		/// <param name="message">error message</param>
		/// <returns>The contained value</returns>
		public T Expect(string message) =>
			Match(
				val => val,
				_ => throw new ExpectException(message)
			);

		/// <summary>
		/// Returns the contained Err value, or throws ExpectErrException if
		/// the Result is Ok
		/// </summary>
		/// <exception cref="ExpectErrException">Thrown if the Result is Ok</exception>
		/// <param name="message">error message</param>
		/// <returns>The contained error</returns>
		public E ExpectErr(string message) =>
			Match(
				_ => throw new ExpectErrException(message),
				err => err
			);

		static string ToStringNullSafe<U>(U value) =>
			value?.ToString() ?? "null";

		public override string ToString() =>
			Match(
				value => $"Ok({ToStringNullSafe(value)})",
				error => $"Err({ToStringNullSafe(error)})"
			);

		/// <summary>
		/// Performs Equality check on the values contained in the results.
		/// If the Results are in different states (eg. Ok and Err) the Equality check
		/// naturally always fails.
		/// </summary>
		/// <param name="other">The other `Result` to compare with this</param>
		/// <returns>True if `this` and `other` are equal</returns>
		public bool Equals(Result<T, E> other) =>
			other.Match(Equals, Equals);

		static bool EqualsNullSafe<U>(U a, U b) =>
			a?.Equals(b) ?? b == null;

		bool Equals(T other) =>
			Match(
				val => EqualsNullSafe(val, other),
				_ => false
			);

		bool Equals(E other) =>
			Match(
				_ => false,
				err => EqualsNullSafe(err, other)
			);

		public override bool Equals(object? obj) =>
			obj switch
			{
				Result<T, E> x => Equals(x),
				ResultOk<T> x => Equals(x.Value),
				ResultErr<E> x => Equals(x.Error),
				_ => false,
			};

		/// <summary>
		/// Calculate the combined hash-code from the contained value and the result state.
		/// Meaning a result of Ok("foo") and Err("foo") does not return the same hashcode
		/// </summary>
		/// <returns>The combined hashcode of the contained value and result state</returns>
		public override int GetHashCode() =>
			Match(
				val => HashCode.Combine(ResultState.Ok, val),
				err => HashCode.Combine(ResultState.Err, err)
			);

		R IResult.MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err) =>
			Match(value => ok(value), error => err(error));

		object? IResult.UnwrapUntyped() =>
			Unwrap();

		object? IResult.UnwrapErrUntyped() =>
			UnwrapErr();

		public static bool operator ==(Result<T, E> lhs, Result<T, E> rhs) =>
			lhs.Equals(rhs);

		public static bool operator !=(Result<T, E> lhs, Result<T, E> rhs) =>
			!(lhs == rhs);

		public static implicit operator Result<T, E>(ResultOk<T> ok) =>
			Ok(ok.Value);

		public static implicit operator Result<T, E>(ResultErr<E> err) =>
			Err(err.Error);
	}
}
