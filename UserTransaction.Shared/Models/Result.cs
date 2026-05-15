namespace UserTransaction.Shared.Models;

public record Result<T>
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; init; }
    public string[] Errors { get; init; } = [];

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(params string[] errors) => new() { IsSuccess = false, Errors = errors };
}

public record Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public string[] Errors { get; init; } = [];
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(params string[] errors) => new() { IsSuccess = false, Errors = errors };
}