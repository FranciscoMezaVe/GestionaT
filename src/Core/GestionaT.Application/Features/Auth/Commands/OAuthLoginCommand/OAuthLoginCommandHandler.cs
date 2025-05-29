using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.OAuthLoginCommand
{
    public class OAuthLoginCommandHandler : IRequestHandler<OAuthLoginCommand, Result<OAuthLoginCommandResponse>>
    {
        private readonly IOAuthServiceFactory _factory;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<OAuthLoginCommand> _logger;
        public OAuthLoginCommandHandler(
            IOAuthServiceFactory factory,
            IUserRepository userRepository,
            IJwtTokenService tokenService,
            IAuthenticationService authenticationService,
            ILogger<OAuthLoginCommand> logger)
        {
            _factory = factory;
            _userRepository = userRepository;
            _jwtTokenService = tokenService;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        public async Task<Result<OAuthLoginCommandResponse>> Handle(OAuthLoginCommand request, CancellationToken cancellationToken)
        {
            var service = _factory.GetService(request.Provider);

            if (service is null)
            {
                return Result.Fail(AppErrorFactory.NotSupported(nameof(request.Provider), request.Provider));
            }

            var userInfo = await service.GetUserInfoAsync(request.AccessToken);

            if (userInfo is null)
            {
                _logger.LogWarning("No se pudo obtener la información del usuario para el proveedor {Provider}", request.Provider);
                return Result.Fail(AppErrorFactory.BadRequest($"No se pudo obtener la información del usuario para el proveedor '{request.Provider}'."));
            }

            if (string.IsNullOrEmpty(userInfo.Email))
            {
                _logger.LogWarning("El proveedor {Provider} no devolvió un correo electrónico válido.", request.Provider);
                return Result.Fail(AppErrorFactory.BadRequest($"El proveedor '{request.Provider}' no devolvió un correo electrónico válido. Active los permisos para poder registrar su cuenta con su correo."));
            }

            var user = await _userRepository.GetByEmailAsync(userInfo.Email);

            if (user is not null)
            {
                return await Login(user.Id, userInfo);
            }

            var userResult = await _authenticationService.RegisterUserOAuthAsync(userInfo);

            if (userResult.IsFailed)
            {
                _logger.LogWarning("Error al registrar o actualizar el usuario: {Errors}", userResult.Errors);
                return Result.Fail(AppErrorFactory.Internal("Error al registrar el usuario."));
            }

            return await Login(userResult, userInfo);
        }

        private async Task<OAuthLoginCommandResponse> Login(Result<Guid> userResult, OAuthUserInfoResult userInfo)
        {
            var userId = userResult.Value;
            var userRoles = await _authenticationService.GetUserRolesAsync(userId);

            var token = _jwtTokenService.GenerateToken(userId, userInfo.Email, userRoles);
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(userId);

            return new OAuthLoginCommandResponse(token, refreshToken);
        }
    }
}
