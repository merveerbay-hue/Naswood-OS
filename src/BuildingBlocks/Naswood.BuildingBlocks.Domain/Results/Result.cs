namespace Naswood.BuildingBlocks.Domain;

/// <summary>
/// Explicit success/failure result for domain and application operations.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException("A successful result cannot contain an error.");
        }

        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException("A failed result must contain an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, null)
    {
        _value = value;
    }

    private Result(Error error)
        : base(false, error)
    {
        _value = default;
    }

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access value of a failed result.");

    public static Result<T> Success(T value) => new(value);

    public new static Result<T> Failure(Error error) => new(error);
}
