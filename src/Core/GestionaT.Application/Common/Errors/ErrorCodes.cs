namespace GestionaT.Application.Common.Errors
{
    public static class ErrorCodes
    {
        public const string NotFound = "NOT_FOUND";
        public const string AlreadyExists = "ALREADY_EXISTS";
        public const string AlreadyOAuthExists = "ALREADY_OAUTH_EXISTS";
        public const string Validation = "VALIDATION_ERROR";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string Conflict = "CONFLICT";
        public const string InternalError = "INTERNAL_ERROR";
        public const string NotSupported = "NOT_SUPPORTED";
        public const string BadRequest = "BAD_REQUEST";
        public const string NotLinked = "NOT_LINKED";
    }
}
