namespace Identity.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public Error? Error { get; private set; }

    private Result() { }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(Error error) => new() { IsSuccess = false, Error = error };
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static Error NotFound(string entity) => new("NOT_FOUND", $"{entity} not found.");
    public static Error Validation(string message) => new("VALIDATION_ERROR", message);
    public static Error Conflict(string message) => new("CONFLICT", message);
    public static Error Unauthorized(string message) => new("UNAUTHORIZED", message);
}
