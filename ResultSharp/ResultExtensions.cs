using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ResultSharp.Prelude;

namespace ResultSharp
{
	public static class ResultExtensions
	{
		public static Result<IEnumerable<T>, IEnumerable<E>> Combine<T, E>(
			this IEnumerable<Result<T, E>> results)
		{
			var data = results as Result<T, E>[] ?? results.ToArray();
			var errors = data.Where(x => x.IsErr);
			if (errors.Any())
			{
				return Err(errors.Select(x => x.UnwrapErr()));
			}
			else
			{
				return Ok(data.Select(x => x.Unwrap()));
			}
		}

		public static Result Combine(
			this IEnumerable<Result> results,
			string errorMessageSeparator) =>
			results
				.Select(x => x.Inner)
				.Combine(
					_ => Unit.Default,
					errs => string.Join(errorMessageSeparator, errs)
				);

		public static Result<IEnumerable<T>> Combine<T>(
			this IEnumerable<Result<T>> results,
			string errorMessageSeparator) =>
			results
				.Select(x => x.Inner)
				.Combine(combineErr: errs => string.Join(errorMessageSeparator, errs));

		public static Result<U, E> Combine<T, U, E>(
			this IEnumerable<Result<T>> results,
			Func<IEnumerable<T>, U> combineOk,
			Func<IEnumerable<string>, E> combineErr) =>
			results
				.Select(x => x.Inner)
				.Combine(combineOk, combineErr);

		public static Result<U, F> Combine<T, U, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> combineOk,
			Func<IEnumerable<E>, F> combineErr) =>
			results
				.Combine()
				.BiMap(combineOk, combineErr);

		public static Result<IEnumerable<T>, F> Combine<T, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<E>, F> combineErr) =>
			results
				.Combine()
				.MapErr(combineErr);

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
