namespace ResultSharp;

/// <summary>
/// Interface for common Result operations
/// </summary>
public interface IResult
{
    /// <summary>
    /// Type of the Ok value
    /// </summary>
    Type OkType { get; }

    /// <summary>
    /// Type of the Error value
    /// </summary>
    Type ErrType { get; }

    /// <summary>
    /// Returns true if the Result is ok
    /// </summary>
    bool IsOk { get; }

    /// <summary>
    /// Returns true if the Result is faulted
    /// </summary>
    bool IsErr { get; }

    /// <summary>
    /// Match the two states of the Result
    /// </summary>
    /// <typeparam name="R">Type of the return value</typeparam>
    /// <param name="ok">Ok match operation</param>
    /// <param name="err">Error match operation</param>
    /// <returns>R</returns>
    R MatchUntyped<R>(Func<object?, R> ok, Func<object?, R> err);

    /// <summary>
    /// Unwrap the ok value as nullable object
    /// </summary>
    object? UnwrapUntyped();

    /// <summary>
    /// Unwrap the error value as nullable object
    /// </summary>
    object? UnwrapErrUntyped();
}
