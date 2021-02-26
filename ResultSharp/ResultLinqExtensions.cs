using System;

namespace ResultSharp
{
	public static class ResultLinqExtensions
	{
		#region Result<T, E>

		public static Result<TResult, TErr> SelectMany<TSource, TCollection, TResult, TErr>(
			this Result<TSource, TErr> result,
			Func<TSource, Result<TCollection, TErr>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector
		) =>
			result.AndThen(a => collectionSelector(a).Map(b => resultSelector(a, b)));

		public static Result<TResult, TErr> SelectMany<TSource, TResult, TErr>(
			this Result<TSource, TErr> result,
			Func<TSource, Result<TResult, TErr>> selector
		) => result.AndThen(selector);

		public static Result<TResult, TErr> Select<TSource, TResult, TErr>(
			this Result<TSource, TErr> result,
			Func<TSource, TResult> selector
		) => result.Map(selector);

		#endregion



		#region Result<T>

		public static Result<TResult> SelectMany<TSource, TCollection, TResult>(
			this Result<TSource> result,
			Func<TSource, Result<TCollection>> collectionSelector,
			Func<TSource, TCollection, TResult> resultSelector
		) =>
			result.AndThen(a => collectionSelector(a).Map(b => resultSelector(a, b)));

		public static Result<TResult> SelectMany<TSource, TResult>(
			this Result<TSource> result,
			Func<TSource, Result<TResult>> selector
		) => result.AndThen(selector);

		public static Result<TResult> Select<TSource, TResult>(
			this Result<TSource> result,
			Func<TSource, TResult> selector
		) => result.Map(selector);

		#endregion
	}
}
