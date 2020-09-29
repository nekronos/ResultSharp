using System;
using System.Collections.Generic;
using System.Text;

namespace ResultSharp
{
	public interface IResult
	{
		Type OkType { get; }
		Type ErrType { get; }

		bool IsOk { get; }
		bool IsErr { get; }

		R MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err);

		object? UnwrapUntyped();
		object? UnwrapErrUntyped();
	}
}
