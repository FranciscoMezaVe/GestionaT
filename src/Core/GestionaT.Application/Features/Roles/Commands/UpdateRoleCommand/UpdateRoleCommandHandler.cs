using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Roles.Commands.UpdateRoleCommand
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRoleCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Role>();
            var role =  repository.Query()
                .FirstOrDefault(r => r.Id == command.Id && r.BusinessId == command.BusinessId);

            if (role == null)
            {
                _logger.LogWarning("No se encontró el rol con ID {RoleId}.", command.Id);
                return Result.Fail(new HttpError("Rol no encontrado.", ResultStatusCode.NotFound));
            }

            _mapper.Map(command.Request, role);

            repository.Update(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Rol actualizado correctamente: {RoleId}", role.Id);
            return Result.Ok();
        }
    }
}