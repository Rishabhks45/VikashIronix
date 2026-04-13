using Microsoft.AspNetCore.Http;
using SharedKernel.Utilities;
using System.Net;

namespace SharedKernel.Utilities.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started. Cannot modify the response.");
                throw;
            }

            var apiResult = MapExceptionToResult(ex, out int httpStatus);

            context.Response.StatusCode = httpStatus;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(apiResult);
        }
    }

    private static Result<object> MapExceptionToResult(Exception exception, out int httpStatus)
    {
        var result = new Result<object>();

        switch (exception)
        {
            case DomainBadRequestException:
                httpStatus = (int)HttpStatusCode.BadRequest;
                return result.ErrorResponse(ErrorCode.BadRequest, exception.Message);

            case DomainNotFoundException:
                httpStatus = (int)HttpStatusCode.NotFound;
                return result.ErrorResponse(ErrorCode.NotFound, exception.Message);

            case DomainUnauthorizedException:
                httpStatus = (int)HttpStatusCode.Unauthorized;
                return result.ErrorResponse(ErrorCode.Unauthorized, exception.Message);

            case DomainForbiddenException:
                httpStatus = (int)HttpStatusCode.Forbidden;
                return result.ErrorResponse(ErrorCode.Forbidden, exception.Message);

            case DomainAlreadyExistsException:
                httpStatus = (int)HttpStatusCode.Conflict;
                return result.ErrorResponse(ErrorCode.AlreadyExists, exception.Message);

            case DomainValidationException:
                httpStatus = (int)HttpStatusCode.BadRequest;
                return result.ErrorResponse(ErrorCode.Validation, exception.Message);

            case BadRequestException:
                httpStatus = (int)HttpStatusCode.BadRequest;
                return result.ErrorResponse(ErrorCode.BadRequest, exception.Message);

            case NotFoundException:
                httpStatus = (int)HttpStatusCode.NotFound;
                return result.ErrorResponse(ErrorCode.NotFound, exception.Message);

            case UnauthorizedException:
                httpStatus = (int)HttpStatusCode.Unauthorized;
                return result.ErrorResponse(ErrorCode.Unauthorized, exception.Message);

            default:
                httpStatus = (int)HttpStatusCode.InternalServerError;
                return result.ErrorResponse(ErrorCode.InternalError, exception.Message);
        }
    }
}

public class DomainBadRequestException : Exception
{
    public DomainBadRequestException(string msg) : base(msg) { }
}

public class DomainNotFoundException : Exception
{
    public DomainNotFoundException(string msg) : base(msg) { }
}

public class DomainUnauthorizedException : Exception
{
    public DomainUnauthorizedException(string msg) : base(msg) { }
}

public class DomainForbiddenException : Exception
{
    public DomainForbiddenException(string msg) : base(msg) { }
}

public class DomainAlreadyExistsException : Exception
{
    public DomainAlreadyExistsException(string msg) : base(msg) { }
}

public class DomainValidationException : Exception
{
    public DomainValidationException(string msg) : base(msg) { }
}

