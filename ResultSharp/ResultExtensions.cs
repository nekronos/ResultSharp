using System;
using System.Collections.Generic;
using System.Linq;
using static ResultSharp.Prelude;

namespace ResultSharp
{
	public static class ResultExtensions
	{
		static string? CombineErrorMessages<R>(
			IEnumerable<R> results,
			string errorMessageSeparator) where R : IResult
		{
			var errors = results
				.Where(x => x.IsErr)
				.Select(x => x.UnwrapErrUntyped().ToString());

			var errorMessage = string.Join(errorMessageSeparator, errors);
			if (string.IsNullOrEmpty(errorMessage))
				return null;
			else
				return errorMessage;
		}

		public static Result Combine(
			this IEnumerable<Result> results,
			string errorMessageSeparator)
		{
			var combinedError = CombineErrorMessages(results, errorMessageSeparator);
			if (string.IsNullOrEmpty(combinedError))
				return Result.Ok();
			else
				return Result.Err(combinedError);
		}

		public static Result<IEnumerable<T>> Combine<T>(
			this IEnumerable<Result<T>> results,
			string errorMessageSeparator)
		{
			var combinedError = CombineErrorMessages(results, errorMessageSeparator);
			if (string.IsNullOrEmpty(combinedError))
			{
				var oks = results
					.Select(x => x.Unwrap())
					.ToArray();
				return Ok<IEnumerable<T>>(oks);
			}
			else
				return Err(combinedError);
		}

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

		public static Result<U, E> AndThenTry<T, U, E>(this Result<T, E> result, Func<T, U> f)
			where E : Exception =>
			result
				.AndThen(x => Try<U, E>(() => f(x)));
	}
}
