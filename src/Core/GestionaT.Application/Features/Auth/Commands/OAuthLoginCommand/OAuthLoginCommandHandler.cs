using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.OAuthLoginCommand
{
    public class OAuthLoginCommandHandler : IRequestHandler<OAuthLoginCommand, Result<OAuthLoginCommandResponse>>
    {
        private readonly IOAuthServiceFactory _factory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<OAuthLoginCommandHandler> _logger;
        public OAuthLoginCommandHandler(
            IOAuthServiceFactory factory,
            IUserRepository userRepository,
            IJwtTokenService tokenService,
            IAuthenticationService authenticationService,
            ILogger<OAuthLoginCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _factory = factory;
            _userRepository = userRepository;
            _jwtTokenService = tokenService;
            _authenticationService = authenticationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
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
                return Result.Fail(AppErrorFactory.Internal($"No se pudo obtener la información del usuario para el proveedor '{request.Provider}'."));
            }

            if (string.IsNullOrEmpty(userInfo.Email))
            {
                _logger.LogWarning("El proveedor {Provider} no devolvió un correo electrónico válido.", request.Provider);
                return Result.Fail(AppErrorFactory.BadRequest($"El proveedor '{request.Provider}' no devolvió un correo electrónico válido. Active los permisos para poder registrar su cuenta con su correo."));
            }

            var user = await _userRepository.GetByEmailAsync(userInfo.Email);

            if (user is not null)
            {
                var oAuthProvider = _unitOfWork.Repository<OAuthProviders>()
                    .Query()
                    .FirstOrDefault(o => o.UserId == user.Id);

                if(oAuthProvider is not null)
                {
                    return await Login(user.Id, userInfo);
                }

                return Result.Fail(AppErrorFactory.NotLinked(request.Provider));
            }

            var userResult = await _authenticationService.RegisterUserOAuthAsync(userInfo);

            if (userResult.IsFailed)
            {
                var errorMessages = string.Join(" | ", userResult.Errors.Select(e => e.Message));
                _logger.LogWarning("Error al registrar usuario: {Errors}", errorMessages);

                return Result.Fail(AppErrorFactory.Internal("Error al registrar el usuario."));
            }

            if (userResult.Value == Guid.Empty)
            {
                _logger.LogError("Se registró el usuario, pero se devolvió un Guid vacío.");
                return Result.Fail(AppErrorFactory.Internal("Error inesperado al registrar el usuario."));
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
