using GestionaT.Application.Common.Errors.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Common.Errors
{
    public class NotFoundStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.NotFound;
        public int GetStatusCode() => StatusCodes.Status404NotFound;
    }

    public class AlreadyExistsStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.AlreadyExists;
        public int GetStatusCode() => StatusCodes.Status409Conflict;
    }

    public class AlreadyOAuthExistsStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.AlreadyOAuthExists;
        public int GetStatusCode() => StatusCodes.Status409Conflict;
    }

    public class ValidationStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.Validation;
        public int GetStatusCode() => StatusCodes.Status422UnprocessableEntity;
    }

    public class UnauthorizedStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.Unauthorized;
        public int GetStatusCode() => StatusCodes.Status401Unauthorized;
    }

    public class ForbiddenStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.Forbidden;
        public int GetStatusCode() => StatusCodes.Status403Forbidden;
    }

    public class ConflictStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.Conflict;
        public int GetStatusCode() => StatusCodes.Status409Conflict;
    }

    public class InternalErrorStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.InternalError;
        public int GetStatusCode() => StatusCodes.Status500InternalServerError;
    }

    public class NotSupportedStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.NotSupported;
        public int GetStatusCode() => StatusCodes.Status422UnprocessableEntity;
    }

    public class BadRequestStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.BadRequest;
        public int GetStatusCode() => StatusCodes.Status400BadRequest;
    }

    public class NotLinkedStatusStrategy : IHttpStatusCodeStrategy
    {
        public bool CanHandle(string errorCode) => errorCode == ErrorCodes.NotLinked;
        public int GetStatusCode() => StatusCodes.Status409Conflict;
    }
}
