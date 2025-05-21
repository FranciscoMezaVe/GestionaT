using FluentResults;
using GestionaT.Domain.Enums;

namespace GestionaT.Application.Common
{
    public class HttpError : Error
    {
        public int StatusCode { get; }

        public HttpError(string message, int statusCode = ResultStatusCode.NotFound) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
