using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Members.Commands.UpdateMemberRoleCommand
{
    public class UpdateMemberRoleCommandHandler : IRequestHandler<UpdateMemberRoleCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateMemberRoleCommandHandler> _logger;

        public UpdateMemberRoleCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<UpdateMemberRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId!.Value;

            var business = await _unitOfWork.Repository<Domain.Entities.Business>().GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                return Result.Fail(new HttpError("Negocio no encontrado.", ResultStatusCode.NotFound));
            }

            //var isOwner = business.OwnerId == currentUserId;
            //if (!isOwner)
            //{
            //    return Result.Fail(new HttpError("Solo el propietario del negocio puede cambiar roles.", ResultStatusCode.Forbidden));
            //}

            var member = _unitOfWork.Repository<Domain.Entities.Members>()
                .Query()
                .FirstOrDefault(m => m.Id == request.MemberId && m.BusinessId == request.BusinessId);

            if (member == null)
            {
                return Result.Fail(new HttpError("Miembro no encontrado.", ResultStatusCode.NotFound));
            }

            if (member.Active != Status.Active)
            {
                return Result.Fail(new HttpError("No se puede cambiar el rol de un miembro inactivo.", ResultStatusCode.Conflict));
            }

            if (member.UserId == currentUserId)
            {
                return Result.Fail(new HttpError("No puedes cambiar tu propio rol si eres el owner.", ResultStatusCode.Forbidden));
            }

            var role = _unitOfWork.Repository<Role>()
                .Query()
                .FirstOrDefault(r => r.Id == request.RoleDto.RoleId && r.BusinessId == request.BusinessId);

            if (role == null)
            {
                return Result.Fail(new HttpError("El rol especificado no existe o no pertenece al negocio.", ResultStatusCode.NotFound));
            }

            if (role.Name == RolesValues.Owner)
            {
                return Result.Fail(new HttpError("No puedes asignar el rol de Owner a un miembro.", ResultStatusCode.Conflict));
            }

            member.RoleId = request.RoleDto.RoleId;
            _unitOfWork.Repository<Domain.Entities.Members>().Update(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Rol del miembro {MemberId} actualizado a {RoleId}", member.Id, member.RoleId);
            return Result.Ok();
        }
    }
}