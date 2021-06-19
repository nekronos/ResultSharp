﻿using System;

namespace ResultSharp
{
	public static class Prelude
	{
		public static ResultOk<Unit> Ok() =>
			new(Unit.Default);

		public static ResultOk<T> Ok<T>(T value) =>
			new(value);

		public static ResultErr<E> Err<E>(E error) =>
			new(error);

		public static Result<T> OkIf<T>(bool condition, T Value, string error) =>
			Result.OkIf(condition, Value, error);

		public static Result<T, E> OkIf<T, E>(bool condition, T value, E error) =>
			Result.OkIf(condition, value, error);

		public static Result<T> OkIf<T>(bool condition, Func<T> getValue, Func<string> getError) =>
			condition
				? Result<T>.Ok(getValue())
				: Result<T>.Err(getError());

		public static Result<T, E> OkIf<T, E>(bool condition, Func<T> getValue, Func<E> getError) =>
			condition
				? Result<T, E>.Ok(getValue())
				: Result<T, E>.Err(getError());

		public static Result OkIf(
			bool condition,
			string error) =>
				OkIf(condition, Unit.Default, error);

		public static Result OkIf(
			bool condition,
			Func<string> getError) =>
				OkIf(condition, () => Unit.Default, getError);

		public static Result<T> ErrIf<T>(bool condition, T value, string error) =>
			Result.ErrIf(condition, value, error);

		public static Result<T, E> ErrIf<T, E>(bool condition, T value, E error) =>
			Result.ErrIf(condition, value, error);

		public static Result<T, E> ErrIf<T, E>(
			bool condition,
			Func<T> getValue,
			Func<E> getError) =>
				OkIf(!condition, getValue, getError);

		public static Result<T> ErrIf<T>(
			bool condition,
			Func<T> getValue,
			Func<string> getError) =>
				OkIf(!condition, getValue, getError);

		public static Result ErrIf(
			bool condition,
			string error) =>
			ErrIf(condition, Unit.Default, error);

		public static Result ErrIf(
			bool condition,
			Func<string> getError) =>
			ErrIf(condition, () => Unit.Default, getError);

		public static Result<T, Exception> Try<T>(Func<T> op) =>
			Result.Try(op);

		public static Result<T, E> Try<T, E>(Func<T> op) where E : Exception =>
			Result.Try<T, E>(op);
	}
}
