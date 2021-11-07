using System;
using System.Collections.Generic;
using System.Linq;
using static ResultSharp.Prelude;

namespace ResultSharp
{
	/// <summary>
	/// Result extension methods
	/// </summary>
	public static class ResultExtensions
	{
		/// <summary>
		/// Combines multiple results into a single result.
		/// The combined result will be faulted if any of the inputs are faulted
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <returns>The combined Result</returns>
		public static Result<IEnumerable<T>, IEnumerable<E>> Combine<T, E>(
			this IEnumerable<Result<T, E>> results)
		{
			var data = results as IReadOnlyCollection<Result<T, E>> ?? results.ToArray();
			var errors = data
				.Where(x => x.IsErr)
				.Select(x => x.Error)
				.ToArray();
			if (errors.Any())
			{
				return Err(errors.AsEnumerable());
			}
			else
			{
				var oks = data
					.Select(x => x.Value)
					.ToArray()
					.AsEnumerable();
				return Ok(oks);
			}
		}

		/// <summary>
		/// Combines multiple results into a single result, joining the errors
		/// with the provided <paramref name="errorMessageSeparator"/>
		/// The combined result will be faulted if any of the inputs are faulted
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="errorMessageSeparator">The string used to join the errors</param>
		/// <returns>The combined Result</returns>
		public static Result Combine(
			this IEnumerable<Result> results,
			string errorMessageSeparator) =>
			results
				.Select(x => x.Inner)
				.Combine(
					_ => Unit.Default,
					errors => string.Join(errorMessageSeparator, errors)
				);

		/// <summary>
		/// Combines multiple results into a single result, joining the errors
		/// with the provided <paramref name="errorMessageSeparator"/>
		/// The combined result will be faulted if any of the inputs are faulted
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="errorMessageSeparator">The string used to join the errors</param>
		/// <returns>The combined Result</returns>
		public static Result<IEnumerable<T>> Combine<T>(
			this IEnumerable<Result<T>> results,
			string errorMessageSeparator) =>
			results
				.Select(x => x.Inner)
				.Combine(err: errors => string.Join(errorMessageSeparator, errors));

		/// <summary>
		/// Combines multiple results into a single result, combining either the
		/// Ok values or the Err values with the provided functions.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="ok">Ok combinator function</param>
		/// <param name="err">Err combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<U, E> Combine<T, U, E>(
			this IEnumerable<Result<T>> results,
			Func<IEnumerable<T>, U> ok,
			Func<IEnumerable<string>, E> err) =>
			results
				.Select(x => x.Inner)
				.Combine(ok, err);

		/// <summary>
		/// Combines multiple results into a single result, combining either the
		/// Ok values or the Err values with the provided functions.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="ok">Ok combinator function</param>
		/// <param name="err">Err combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<U, F> Combine<T, U, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> ok,
			Func<IEnumerable<E>, F> err) =>
			results
				.Combine()
				.BiMap(ok, err);

		/// <summary>
		/// Combines multiple results into a single result, combining the
		/// Err values with the provided function.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="err">Err combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<IEnumerable<T>, F> Combine<T, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<E>, F> err) =>
			results
				.Combine()
				.MapErr(err);

		/// <summary>
		/// Combines multiple results into a single result, combining the
		/// Ok values with the provided function.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="ok">Ok combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<U, IEnumerable<E>> Combine<T, U, E>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> ok) =>
			results
				.Combine()
				.Map(ok);

		public static Result<U, E> AndThenTry<T, U, E>(this Result<T, E> result, Func<T, U> fn)
			where E : Exception =>
			result
				.AndThen(x => Try<U, E>(() => fn(x)));

		/// <inheritdoc cref="Result{T, E}.And{U}(Result{U, E})" />
		public static Result And<T>(this Result<T, string> result, Result other) =>
			result
				.Match(_ => other, Result.Err);

		/// <inheritdoc cref="Result{T, E}.And{U}(Result{U, E})" />
		public static Result<U> And<T, U>(this Result<T, string> result, Result<U> other) =>
			result
				.Match(_ => other, Result.Err<U>);

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		public static Result<U> AndThen<T, U>(this Result<T, string> result, Func<T, Result<U>> fn) =>
			result
				.Match(fn, Result.Err<U>);

		/// <inheritdoc cref="Result{T, E}.AndThen{U}(Func{T, Result{U, E}})" />
		public static Result AndThen<T>(this Result<T, string> result, Func<T, Result> fn) =>
			result
				.Match(fn, Result.Err);
	}
}
