using System;

namespace ResultSharp
{
	public static class ResultLinqExtensions
	{
		#region Result<T, E>

		public static Result<V, E> SelectMany<T, U, V, E>(
			this Result<T, E> result,
			Func<T, Result<U, E>> bind,
			Func<T, U, V> project) =>
			result.AndThen(a => bind(a).Map(b => project(a, b)));

		public static Result<U, E> SelectMany<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> bind) => result.AndThen(bind);

		public static Result<U, E> Select<T, U, E>(this Result<T, E> result, Func<T, U> project) => result.Map(project);

		#endregion

		#region Result<T>

		public static Result<V> SelectMany<T, U, V>(
			this Result<T> result,
			Func<T, Result<U>> bind,
			Func<T, U, V> project) =>
			result.AndThen(a => bind(a).Map(b => project(a, b)));

		public static Result<U> SelectMany<T, U>(this Result<T> result, Func<T, Result<U>> bind) =>
			result.AndThen(bind);

		public static Result<U> Select<T, U>(this Result<T> result, Func<T, U> project) =>
			result.Map(project);

		#endregion
	}
}
