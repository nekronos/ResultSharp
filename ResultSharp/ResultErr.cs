namespace ResultSharp
{
	public readonly struct ResultErr<E> where E : notnull
	{
		internal readonly E Error;
		internal ResultErr(E error) =>
			Error = error;
	}
}
