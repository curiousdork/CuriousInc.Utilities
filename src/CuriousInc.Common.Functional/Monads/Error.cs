namespace CuriousInc.Common.Functional.Monads;

public enum ErrorType
{
    None = 0,
    Validation,
    Exception,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Internal,
    IsNull
}


public record Error : Status
{
    public string Message { get; init; } = string.Empty;
    public ErrorType Type { get; init; } = ErrorType.None;
    public Option<string> Field { get; init; } = Option<string>.None;
    public Option<Exception> Exception { get; init; } = Option<Exception>.None;
    
    public static implicit operator Error(Exception exception) => new(exception, ErrorType.Exception);

    public bool HasError { get; init; } = false;
    
    private Error(string message, ErrorType type)
    {
        Message = message;
        Type = type;
        HasError = true;
    }
    
    private Error(string message, ErrorType type, string field) : this(message, type)
    {
        Field = field;
    }

    private Error(Exception exception, ErrorType type)
    {
        Exception = exception;
        Type = type;
        HasError = true;
    }
    
    public static Error None => new(string.Empty, ErrorType.None);
    
    public static Error New(Exception exception) => new(exception, ErrorType.Exception);
    public static Error New(string message, ErrorType errorType) => new(message, errorType);
    public static Error New(string field, string message, ErrorType errorType) => new(message, errorType, field);
}