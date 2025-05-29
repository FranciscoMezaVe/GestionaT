using GestionaT.Application.Common.Errors.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Common.Errors
{
    public class HttpStatusCodeResolver
    {
        private readonly IEnumerable<IHttpStatusCodeStrategy> _strategies;

        public HttpStatusCodeResolver(IEnumerable<IHttpStatusCodeStrategy> strategies)
        {
            _strategies = strategies;
        }

        public int Resolve(string errorCode)
        {
            return _strategies.FirstOrDefault(s => s.CanHandle(errorCode))?.GetStatusCode()
                ?? StatusCodes.Status400BadRequest;
        }
    }
}
