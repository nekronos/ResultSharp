using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;
using ResultSharp;

namespace FluentAssertions.ResultSharp
{
	public static class ResultExtensions
	{
		public static ResultAssertions Should(this Result result) => new(result);

		public static ResultAssertions<T> Should<T>(this Result<T> result) => new(result);

		public static ResultAssertions<T, E> Should<T, E>(this Result<T, E> result) => new(result);
	}

	public abstract class ResultAssertionsBase<TResult, TAssertions> : ReferenceTypeAssertions<TResult, TAssertions>
		where TResult : notnull
		where TAssertions : ReferenceTypeAssertions<TResult, TAssertions>
	{
		protected ResultAssertionsBase(TResult subject) : base(subject) { }
	}



	[DebuggerNonUserCode]
	public sealed class ResultAssertions : ResultAssertionsBase<Result, ResultAssertions> // : ReferenceTypeAssertions<Result, ResultAssertions>
	{
		public ResultAssertions(Result subject) : base(subject)
		{
		}

		protected override string Identifier => nameof(Result);

		public AndConstraint<ResultAssertions> BeOk(string because = "")
		{
			var errorMessage = Subject.Match(() => string.Empty, err => err);

			Execute
				.Assertion
				.BecauseOf(because)
				.ForCondition(Subject.IsOk)
				.FailWith($"Expected result to be ok{{reason}}, but it is faulted with '{errorMessage}'");

			return new(this);
		}

		public AndConstraint<ResultAssertions> BeErr(string because = "")
		{
			Execute
				.Assertion
				.BecauseOf(because)
				.ForCondition(Subject.IsErr)
				.FailWith("Expected result to be faulted{reason}, but it is ok");

			return new(this);
		}

		public AndConstraint<ResultAssertions> HaveError(string error, string because = "")
		{
			Execute
				.Assertion
				.BecauseOf(because)
				.ForCondition(Subject.Match(() => false, err => err.Equals(error)))
				.FailWith($"Expected result to be faulted with '{error}'{{reason}}, but got '{Subject}'");

			return new(this);
		}
	}

	[DebuggerNonUserCode]
	public sealed class ResultAssertions<T> : ResultAssertionsBase<Result<T>, ResultAssertions<T>>
	{
		public ResultAssertions(Result<T> subject) : base(subject)
		{
		}

		protected override string Identifier => nameof(Result<T>);

		public AndConstraint<ResultAssertions<T>> Be(Result<T> expected, string because = "", params object[] becauseArgs) =>
			this.BeAssertion(expected, because, becauseArgs);

	}

	[DebuggerNonUserCode]
	public sealed class ResultAssertions<T, E> : ResultAssertionsBase<Result<T, E>, ResultAssertions<T, E>>
	{
		public ResultAssertions(Result<T, E> subject) : base(subject)
		{
		}

		protected override string Identifier => nameof(Result<T, E>);

		public AndConstraint<ResultAssertions<T, E>> Be(Result<T, E> expected, string because = "", params object[] becauseArgs) =>
			this.BeAssertion(expected, because, becauseArgs);
	}

	internal static class AssertionFunctions
	{
		public static AndConstraint<TResultAssertion> BeAssertion<TResult, TResultAssertion>(
			this TResultAssertion resultAssertions,
			TResult expected,
			string because = "",
			params object[] becauseArgs)
			where TResult : notnull
			where TResultAssertion : ResultAssertionsBase<TResult, TResultAssertion>
		{
			Execute
				.Assertion
				.BecauseOf(because, becauseArgs)
				.ForCondition(resultAssertions.Subject.Equals(expected))
				.FailWith($"Expected result to be {expected}{{reason}}, but got {resultAssertions.Subject}.");

			return new(resultAssertions);
		}
	}
}
