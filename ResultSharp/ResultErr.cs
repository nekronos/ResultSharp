﻿namespace ResultSharp;

/// <summary>
/// Intermediate type holding the error value of a Result
/// </summary>
/// <typeparam name="E">The type of the error</typeparam>
public readonly struct ResultErr<E>
{
    internal E Error { get; }
    internal ResultErr(E error) =>
        Error = error;
}