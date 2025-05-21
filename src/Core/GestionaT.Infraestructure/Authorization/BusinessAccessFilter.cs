using GestionaT.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GestionaT.Infraestructure.Authorization
{
    public class BusinessAccessFilter : IAsyncActionFilter
    {
        private readonly string _routeParam;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<BusinessAccessFilter> _logger;

        public BusinessAccessFilter(string routeParam, ICurrentUserService currentUserService, ILogger<BusinessAccessFilter> logger)
        {
            _routeParam = routeParam;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue(_routeParam, out var value) || value is not Guid businessId)
            {
                context.Result = new BadRequestObjectResult($"No se encontró el parámetro '{_routeParam}' en la ruta.");
                return;
            }

            if (!_currentUserService.BusinessIds.Contains(businessId.ToString()))
            {
                _logger.LogWarning("El usuario {userId} no tiene acceso al negocio con id {businessId}", _currentUserService.UserId, businessId);
                context.Result = new ForbidResult(); // 403
                return;
            }

            await next();
        }
    }
}
