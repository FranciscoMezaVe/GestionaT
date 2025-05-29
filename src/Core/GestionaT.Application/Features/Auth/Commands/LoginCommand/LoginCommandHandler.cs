using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
    {
        private readonly IAuthenticationService _authentication;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<LoginCommandHandler> _logger;
        public LoginCommandHandler(IAuthenticationService authentication, IJwtTokenService tokenGenerator, ILogger<LoginCommandHandler> logger)
        {
            _authentication = authentication;
            _jwtTokenService = tokenGenerator;
            _logger = logger;
        }
        public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (!await _authentication.Authenticate(request.Email, request.Password))
            {
                _logger.LogWarning("Inicio de sesion invalido para el usuario {Email}", request.Email);
                return Result.Fail(AppErrorFactory.Unauthorized("Credenciales invalidas"));
            }
            Guid userId = await _authentication.GetUserIdAsync(request.Email);
            var userRoles = await _authentication.GetUserRolesAsync(userId);
            string tokenString = _jwtTokenService.GenerateToken(userId, request.Email, userRoles);
            string refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(userId);
            return new LoginCommandResponse(tokenString, refreshToken);         
        }
    }
}
