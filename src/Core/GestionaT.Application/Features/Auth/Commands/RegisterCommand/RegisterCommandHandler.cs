using AutoMapper;
using FluentResults;
using GestionaT.Application.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Auth.Commands.RegisterCommand
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterCommand> _logger;

        public RegisterCommandHandler(IAuthenticationService authenticationService, IMapper mapper, ILogger<RegisterCommand> logger)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registrando usuario {Email}", command.request.Email);

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
