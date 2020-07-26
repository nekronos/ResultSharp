namespace ResultSharp
{
	public readonly struct ResultOk<T>
	{
		internal readonly T Value;
		internal ResultOk(T value) =>
			Value = value;
	}
}
