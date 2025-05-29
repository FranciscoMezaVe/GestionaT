using FluentResults;

namespace GestionaT.Application.Common.Errors
{
    public static class AppErrorFactory
    {
        public static Error NotFound(string entity, object id) =>
            Create(ErrorCodes.NotFound, $"{entity} con ID '{id}' no fue encontrado.");

        public static Error AlreadyExists(string entity, string? detail = null) =>
            Create(ErrorCodes.AlreadyExists, $"{entity} ya existe.", detail);

        public static Error AlreadyOAuthExists(string entity, string provider, string? detail = null) =>
            Create(ErrorCodes.AlreadyOAuthExists, $"{entity} ya existe con el proveedor {provider}.", detail);

        public static Error Validation(string field, string message) =>
            Create(ErrorCodes.Validation, message, null, field);

        public static Error Unauthorized(string reason) =>
            Create(ErrorCodes.Unauthorized, reason);

        public static Error Forbidden(string reason) =>
            Create(ErrorCodes.Forbidden, reason);

        public static Error Conflict(string reason) =>
            Create(ErrorCodes.Conflict, reason);

        public static Error Internal(string detail) =>
            Create(ErrorCodes.InternalError, "Error interno del servidor.", detail);

        public static Error NotSupported(string field, string unsupportedValue, string? detail = null) =>
            Create(
                ErrorCodes.NotSupported,
                $"El valor '{unsupportedValue}' no es soportado para el campo '{field}'.",
                detail,
                field
            );

        public static Error BadRequest(string message, string? detail = null) =>
            Create(ErrorCodes.BadRequest, message, detail);

        public static Error NotLinked(string provider, string? detail = null) =>
            Create(ErrorCodes.NotLinked, $"La cuenta no esta vinculada al proveedor {provider}", detail);

        private static Error Create(string code, string message, string? detail = null, string? field = null)
        {
            var error = new Error(message).WithMetadata(MetaDataErrorValues.Code, code);

            if (!string.IsNullOrEmpty(detail))
                error.WithMetadata(MetaDataErrorValues.Detail, detail);

            if (!string.IsNullOrEmpty(field))
                error.WithMetadata(MetaDataErrorValues.Field, field);

            return error;
        }
    }
}
