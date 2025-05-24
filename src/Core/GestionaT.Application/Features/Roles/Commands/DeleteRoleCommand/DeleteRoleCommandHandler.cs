using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Roles.Commands.DeleteRoleCommand
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Role>();
            var role = repository.Query()
                .FirstOrDefault(r => r.Id == request.Id && r.BusinessId == request.BusinessId);

            if (role == null)
            {
                _logger.LogWarning("No se encontró el rol con ID {RoleId} para eliminación.", request.Id);
                return Result.Fail(new HttpError("Rol no encontrado.", ResultStatusCode.NotFound));
            }

            if (role.Name == RolesValues.Owner)
            {
                _logger.LogWarning("No se puede eliminar el rol de Owner.");
                return Result.Fail(new HttpError("No se puede eliminar el rol de Owner.", ResultStatusCode.Conflict));
            }

            if(role.Name == RolesValues.Worker)
            {
                _logger.LogWarning("No se puede eliminar el rol de Worker.");
                return Result.Fail(new HttpError("No se puede eliminar el rol de Worker.", ResultStatusCode.Conflict));
            }

            var isAssigned = await _unitOfWork.Repository<Domain.Entities.Members>()
                .AnyAsync(u => u.RoleId == role.Id);

            if (isAssigned)
            {
                return Result.Fail(new HttpError("No se puede eliminar el rol porque está asignado a usuarios.", ResultStatusCode.Conflict));
            }

            // Soft delete
            repository.Remove(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Rol marcado como eliminado: {RoleId}", role.Id);
            return Result.Ok();
        }
    }
}