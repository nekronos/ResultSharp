using System;
using System.Collections.Generic;
using System.Linq;
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

		public static Result<U, F> Combine<T, U, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> combineOk,
			Func<IEnumerable<E>, F> combineErr) =>
			results
				.Combine()
				.Match<Result<U, F>>(val => combineOk(val), err => combineErr(err));

		public static Result<IEnumerable<T>, F> Combine<T, E, F>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<E>, F> combineErr) =>
			results
				.Combine(val => val, err => combineErr(err));

		public static Result<U, IEnumerable<E>> Combine<T, U, E>(
			this IEnumerable<Result<T, E>> results,
			Func<IEnumerable<T>, U> combineOk) =>
			results
				.Combine(val => combineOk(val), err => err);

		public static Result<IEnumerable<T>, IEnumerable<E>> CombineMany<T, E>(
			this IEnumerable<Result<IEnumerable<T>, IEnumerable<E>>> results) =>
			results
				.Combine()
				.Match<Result<IEnumerable<T>, IEnumerable<E>>>(
					val => Ok(val.SelectMany(x => x)),
					err => Err(err.SelectMany(x => x)));

		public static Result<U, E> AndThenTry<T, U, E>(
			this Result<T, E> result,
			Func<T, U> f) where E : Exception =>
			result
				.AndThen(x => Try<U, E>(() => f(x)));
	}
}
