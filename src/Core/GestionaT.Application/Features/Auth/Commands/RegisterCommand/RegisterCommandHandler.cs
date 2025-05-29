using AutoMapper;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.RegisterCommand
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterCommand> _logger;

        public RegisterCommandHandler(IAuthenticationService authenticationService, IMapper mapper, ILogger<RegisterCommand> logger, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registrando usuario {Email}", command.request.Email);

            var user = await _userRepository.GetByEmailAsync(command.request.Email);

            if (user is not null)
            {
                var providerUser = _unitOfWork.Repository<OAuthProviders>().Query()
                    .FirstOrDefault(p => p.UserId == user.Id);

                if (providerUser is not null)
                {
                    _logger.LogWarning("El usuario {Email} ya está registrado con el proveedor {Provider}.", command.request.Email, providerUser.ExternalProvider);
                    return Result.Fail(AppErrorFactory.AlreadyExists(nameof(command.request.Email), $"El usuario ya está registrado con el proveedor '{providerUser.ExternalProvider}'."));
                }

                _logger.LogWarning("El usuario {Email} ya existe pero no está vinculado a un proveedor OAuth.", command.request.Email);
                return Result.Fail(AppErrorFactory.AlreadyExists(nameof(command.request.Email), "El usuario ya esta registrado con ese correo."));
            }

            var result = await _authenticationService.RegisterUserAsync(command.request.Email, command.request.UserName, command.request.Password);

            if (result.IsFailed)
            {
                _logger.LogWarning("Error al registrar usuario {Email}: {Errors}",
                    command.request.Email,
                    string.Join(", ", result.Errors.Select(e => e.Message)));

                return result;
            }

            _logger.LogInformation("Usuario {Email} registrado correctamente con ID {UserId}", command.request.Email, result.Value);

            return result;
        }
    }
}
