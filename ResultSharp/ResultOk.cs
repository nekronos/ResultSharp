namespace ResultSharp
{
	public readonly struct ResultOk<T> where T : notnull
	{
		internal readonly T Value;
		internal ResultOk(T value) =>
			Value = value;
	}
}
