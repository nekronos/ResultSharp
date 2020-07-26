namespace ResultSharp
{
	public readonly struct ResultErr<E>
	{
		internal readonly E Error;
		internal ResultErr(E error) =>
			Error = error;
	}
}
