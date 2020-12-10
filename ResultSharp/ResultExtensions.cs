using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			var data = results as Result<T, E>[] ?? results.ToArray();
			var errors = data
				.Where(x => x.IsErr)
				.Select(x => x.UnwrapErr())
				.ToArray();
			if (errors.Any())
			{
				return Err(errors.AsEnumerable());
			}
			else
			{
				var oks = data
					.Select(x => x.Unwrap())
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
					errs => string.Join(errorMessageSeparator, errs)
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
				.Combine(combineErr: errs => string.Join(errorMessageSeparator, errs));

		/// <summary>
		/// Combines multiple results into a single result, combining either the
		/// Ok values or the Err values with the provided functions.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="combineOk">Ok combinator function</param>
		/// <param name="combineErr">Err combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<U, E> Combine<T, U, E>(
			this IEnumerable<Result<T>> results,
			Func<IEnumerable<T>, U> combineOk,
			Func<IEnumerable<string>, E> combineErr) =>
			results
				.Select(x => x.Inner)
				.Combine(combineOk, combineErr);

		/// <summary>
		/// Combines multiple results into a single result, combining either the
		/// Ok values or the Err values with the provided functions.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="combineOk">Ok combinator function</param>
		/// <param name="combineErr">Err combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<U, F> Combine<T, U, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> combineOk,
			Func<IEnumerable<E>, F> combineErr) =>
			results
				.Combine()
				.BiMap(combineOk, combineErr);

		/// <summary>
		/// Combines multiple results into a single result, combining the
		/// Err values with the provided function.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="combineErr">Err combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<IEnumerable<T>, F> Combine<T, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<E>, F> combineErr) =>
			results
				.Combine()
				.MapErr(combineErr);

		/// <summary>
		/// Combines multiple results into a single result, combining the
		/// Ok values with the provided function.
		/// </summary>
		/// <param name="results">The Results to be combined</param>
		/// <param name="combineOk">Ok combinator function</param>
		/// <returns>The combined Result</returns>
		public static Result<U, IEnumerable<E>> Combine<T, U, E>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> combineOk) =>
			results
				.Combine()
				.Map(combineOk);

		public static Result<IEnumerable<T>, IEnumerable<E>> CombineMany<T, E>(
			this IEnumerable<Result<IEnumerable<T>, IEnumerable<E>>> results) =>
			results
				.Combine()
				.BiMap(EnumerableExtensions.Flatten, EnumerableExtensions.Flatten);

		public static Result<U, E> AndThenTry<T, U, E>(this Result<T, E> result, Func<T, U> f)
			where E : Exception =>
			result
				.AndThen(x => Try<U, E>(() => f(x)));

		public static Result And<T>(this Result<T, string> result, Result other) =>
			result
				.Match(_ => other, Result.Err);

		public static Result<U> And<T, U>(this Result<T, string> result, Result<U> other) =>
			result
				.Match(_ => other, Result.Err<U>);

		public static Result<U> AndThen<T, U>(this Result<T, string> result, Func<T, Result<U>> op) =>
			result
				.Match(val => op(val), Result.Err<U>);

		public static Result AndThen<T>(this Result<T, string> result, Func<T, Result> op) =>
			result
				.Match(op, Result.Err);
	}

	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> nestedEnumerable) =>
			nestedEnumerable.SelectMany(x => x);
	}
}
