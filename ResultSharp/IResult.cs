namespace ResultSharp
{
	public interface IResult
	{
		bool IsOk { get; }
		bool IsErr { get; }
		object UnwrapErrUntyped();
	}
}
