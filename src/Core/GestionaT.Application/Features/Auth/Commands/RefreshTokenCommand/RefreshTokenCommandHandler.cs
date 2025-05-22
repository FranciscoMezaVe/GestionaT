using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Features.Auth.Commands.LoginCommand;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.RefreshTokenCommand
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenCommandResponse>>
    {
        private readonly IAuthenticationService _authentication;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(IAuthenticationService authentication, IJwtTokenService jwtTokenService, ILogger<RefreshTokenCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _authentication = authentication;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var storedRefreshToken = _unitOfWork.Repository<RefreshToken>()
                .Query()
                .FirstOrDefault(x => x.Token == request.RefreshToken);

            if (storedRefreshToken is null || !storedRefreshToken.IsValid())
            {
                _logger.LogWarning("El refresh token {RefreshToken} no es valido", request.RefreshToken);
                return Result.Fail(new HttpError("El refresh token no es valido.", ResultStatusCode.Unauthorized));
            }

            Guid userId = storedRefreshToken.UserId;
            if (!await _authentication.IsExistsUserByIdAsync(userId))
            {
                _logger.LogWarning("El usuario {userId} no existe", userId);
                return Result.Fail(new HttpError("El usuario no existe.", ResultStatusCode.Unauthorized));
            }

            _logger.LogWarning("El refresh token {RefreshToken} es valido", request.RefreshToken);
            _logger.LogWarning("Generando nuevas credenciales.");

            string userEmail = await _authentication.GetUserEmailAsync(userId);
            var userRoles = await _authentication.GetUserRolesAsync(userId);

            string tokenString = _jwtTokenService.GenerateToken(userId, userEmail, userRoles);
            string refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(userId);

            return new RefreshTokenCommandResponse(tokenString, refreshToken);
        }
    }
}
