using System;
using System.Collections.Generic;
using System.Text;

namespace ResultSharp
{
	internal static class Messages
	{
		public static string UnwrapCalledOnAnOkResult { get; } =
			"Unwrap was called on an ok result";

		public static string UnwrapCalledOnAFailedResult { get; } =
			"Unwrap was called on a failed result";
	}
}
