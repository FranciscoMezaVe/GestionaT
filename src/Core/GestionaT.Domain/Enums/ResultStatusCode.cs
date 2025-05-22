namespace GestionaT.Domain.Enums
{
    public static class ResultStatusCode
    {
        public const int Ok = 200;
        public const int Created = 201;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int InternalError = 500;
        public const int UnprocesableContent = 422;
        public const int NoContent = 204;
    }
}
