using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace ResultSharp
{
	public struct Unit { }

	[Serializable]
	public readonly partial struct Result : ISerializable
	{
		static Unit unit { get; } = new Unit();

		readonly Result<Unit, string> Inner;

		Result(Result<Unit, string> inner) =>
			Inner = inner;

		Result(
			SerializationInfo info,
			StreamingContext context)
		{
			Inner = (Result<Unit, string>)info.GetValue(nameof(Inner), typeof(string));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context) =>
			info.AddValue(nameof(Inner), Inner);

		public static Result Ok() =>
			new Result(unit);

		public static Result Err(string error) =>
			new Result(error);

		public static implicit operator Result(ResultOk<Unit> resultOk) =>
			Ok();

		public static implicit operator Result(ResultErr<string> resultErr) =>
			Err(resultErr.Error);

		public static implicit operator Result(Result<Unit, string> result) =>
			new Result(result);

		public static implicit operator Result<Unit, string>(Result result) =>
			result.Inner;

		[Pure]
		public Result<T> Map<T>(Func<T> op) =>
			Inner.Map(_ => op());

		[Pure]
		public Result And(Result result) =>
			Inner.And(result.Inner);

		[Pure]
		public Result AndThen(Func<Result> op) =>
			Inner.AndThen<Unit>(_ => op());

		[Pure]
		public Result Or(Result result) =>
			Inner.Or(result.Inner);

		[Pure]
		public void Unwrap() =>
			Inner.Unwrap();

		[Pure]
		public string UnwrapErr() =>
			Inner.UnwrapErr();

		[Pure]
		public void Expect(string msg) =>
			Inner.Expect(msg);

		[Pure]
		public string ExpectErr(string msg) =>
			Inner.ExpectErr(msg);

		public override string ToString() =>
			Inner.Match(_ => "Ok()", err => $"Err({err})");

		public override bool Equals(object obj) =>
			Inner.Equals(obj);

		public override int GetHashCode() =>
			Inner.GetHashCode();
	}
}
