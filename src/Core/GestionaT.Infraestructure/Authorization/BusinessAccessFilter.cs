using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GestionaT.Shared.Abstractions;
using GestionaT.Application.Interfaces.Repositories;

namespace GestionaT.Infraestructure.Authorization
{
    public class BusinessAccessFilter : IAsyncActionFilter
    {
        private readonly string _routeParam;
        private readonly IBusinessRepository _businessRepository;
        private readonly ILogger<BusinessAccessFilter> _logger;
        private readonly ICurrentUserService _currentUserService;

        public BusinessAccessFilter(string routeParam, IBusinessRepository businessRepository, ILogger<BusinessAccessFilter> logger, ICurrentUserService currentUserService)
        {
            _routeParam = routeParam;
            _businessRepository = businessRepository;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue(_routeParam, out var value) || value is not Guid businessId)
            {
                context.Result = new BadRequestObjectResult($"No se encontró el parámetro '{_routeParam}' en la ruta.");
                return;
            }

            Guid userId = _currentUserService.UserId!.Value;

            if (!_businessRepository.GetBusinessIdsAccessibleByUser(userId).Contains(businessId))
            {
                _logger.LogWarning("El usuario {userId} no tiene acceso al negocio con id {businessId}", userId, businessId);
                context.Result = new ForbidResult(); // 403
                return;
            }

            await next();
        }
    }
}
