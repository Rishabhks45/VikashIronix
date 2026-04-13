namespace SharedKernel.Utilities;

public enum ErrorCode
{
    None = 0,

    // 400 – Client Errors
    Validation = 100,
    BadRequest = 101,
    InvalidInput = 102,
    Conflict = 103,              
    LimitExceeded = 104,

    // 401 / 403 – Auth Errors
    Unauthorized = 401,
    Forbidden = 403,

    // 404 – Missing Resources
    NotFound = 300,

    // 409 – Data constraints
    AlreadyExists = 400,         

    // 500 – Server Errors
    InternalError = 500,
    DatabaseError = 501,
    ExternalServiceError = 502,
    Unknown = 999
}

public class Result<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public ErrorCode ErrorCode { get; set; }
    public List<AppError> Errors { get; set; } = new();

    public Result<T> SuccessResponse(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data, ErrorCode = ErrorCode.None };

    public Result<T> ErrorResponse(ErrorCode code, string message, List<AppError>? errors = null)
    {
        var errorList = errors ?? new List<AppError>();
        errorList.Add(new AppError(code, message));
        return new Result<T>
        {
            Success = false,
            Message = message,
            ErrorCode = code,
            Errors = errorList
        };
    }
}

public class AppError
{
    #region # Init

    public AppError() { }

    public AppError(ErrorCode code, string error)
    {
        Code = code;
        Error = error;
    }

    #endregion

    public ErrorCode Code { get; set; }
    public string Error { get; set; } = string.Empty;
}

