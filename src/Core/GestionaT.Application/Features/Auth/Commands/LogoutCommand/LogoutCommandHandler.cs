using FluentResults;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.LogoutCommand
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(IJwtTokenService jwtTokenService, ILogger<LogoutCommandHandler> logger, ICurrentUserService currentUserService)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Intento de cierre de sesión fallido: ID de usuario no encontrado.");
                return Result.Fail("No se pudo encontrar el ID de usuario para cerrar sesión.");
            }

            var isRemove = await _jwtTokenService.RemoveRefreshTokenAsync(userId!.Value);

            if(!isRemove)
            {
                _logger.LogWarning("Intento de cierre de sesión fallido: No se pudo invalidar el token de actualización.");
                return Result.Fail("No se pudo invalidar el token de actualización.");
            }

            return Result.Ok();
        }
    }
}
