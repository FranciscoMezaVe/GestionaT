using System.Security.Claims;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Infraestructure.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id)
            ? id : null;

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        public List<string> BusinessIds =>
            _httpContextAccessor.HttpContext?.User?.FindAll(ClaimsTypeExtensions.Bussinesses)
                ?.Select(c => c.Value).ToList() ?? new();
    }
}
