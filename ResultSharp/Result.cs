using System;
using System.Runtime.Serialization;

namespace ResultSharp
{
	[Serializable]
	public readonly partial struct Result :
		ISerializable,
		IEquatable<Result>,
		IResult
	{
		readonly Result<Unit, string> Inner;

		Result(Result<Unit, string> inner) =>
			Inner = inner;

		Result(
			SerializationInfo info,
			StreamingContext context)
		{
			Inner = (Result<Unit, string>)info.GetValue(nameof(Inner), typeof(Result<Unit, string>));
		}

		public void GetObjectData(
			SerializationInfo info,
			StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		static readonly Result DefaultOk =
			new Result(Unit.Default);

		public static Result Ok() =>
			DefaultOk;

		public static Result Err(string error) =>
			new Result(error);

		public bool IsOk => Inner.IsOk;

		public bool IsErr => Inner.IsErr;

		public Result<T> Map<T>(Func<T> op) =>
			Inner.Map(_ => op());

		public TRet Match<TRet>(Func<TRet> ok, Func<string, TRet> err) =>
			Inner.Match(_ => ok(), err);

		public Result And(Result result) =>
			Inner.And(result.Inner);

		public Result AndThen(Func<Result> op) =>
			Inner.AndThen<Unit>(_ => op());

		public Result Or(Result result) =>
			Inner.Or(result.Inner);

		public void Unwrap() =>
			Inner.Unwrap();

		public string UnwrapErr() =>
			Inner.UnwrapErr();

		public void Expect(string msg) =>
			Inner.Expect(msg);

		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		public override string ToString() =>
			Inner.Match(_ => "Ok()", err => $"Err({err})");

		public bool Equals(Result result) =>
			Inner.Equals(result.Inner);

		public override bool Equals(object obj) =>
			obj switch
			{
				Result x => Equals(x),
				ResultOk<Unit> x => Equals(x),
				ResultErr<string> x => Equals(x),
				_ => false,
			};

		public override int GetHashCode() =>
			Inner.GetHashCode();

		object IResult.UnwrapErrUntyped() =>
			UnwrapErr();

		public static implicit operator Result(ResultOk<Unit> _) =>
			DefaultOk;

		public static implicit operator Result(ResultErr<string> resultErr) =>
			Err(resultErr.Error);

		public static implicit operator Result(Result<Unit, string> result) =>
			new Result(result);

		public static implicit operator Result<Unit, string>(Result result) =>
			result.Inner;
	}
}
