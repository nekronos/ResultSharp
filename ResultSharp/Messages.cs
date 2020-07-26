namespace ResultSharp
{
	internal static class Messages
	{
		public static string UnwrapErrCalledOnAnOkResult { get; } =
			"UnwrapErr was called on an ok result";

		public static string UnwrapCalledOnAFaultedResult { get; } =
			"Unwrap was called on a faulted result";
	}
}
