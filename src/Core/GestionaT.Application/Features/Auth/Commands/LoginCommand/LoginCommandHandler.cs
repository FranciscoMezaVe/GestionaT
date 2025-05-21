using FluentResults;
using GestionaT.Application.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.LoginCommand
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<string>>
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
        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _authentication.Authenticate(request.Email, request.Password))
                {
                    Guid userId = await _authentication.GetUserIdAsync(request.Email);
                    var userRoles = await _authentication.GetUserRolesAsync(userId);
                    return _jwtTokenService.GenerateToken(userId, request.Email, userRoles);
                }
                return Result.Fail<string>("Invalid credentials");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error mientras se iniciaba sesion");
                return Result.Fail<string>(new Error("Error mientras se iniciaba sesion").CausedBy(e));
            }
        }
    }
}
