namespace ResultSharp;

/// <summary>
/// Intermediate type holding the ok part of a Result
/// </summary>
/// <typeparam name="T">Type of the Ok value</typeparam>
public readonly struct ResultOk<T>
{
    internal T Value { get; }
    internal ResultOk(T value) =>
        Value = value;
}