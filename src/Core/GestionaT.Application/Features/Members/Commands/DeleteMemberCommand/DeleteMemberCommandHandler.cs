using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Members.Commands.DeleteMemberCommand
{
    public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteMemberCommandHandler> _logger;

        public DeleteMemberCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<DeleteMemberCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId!.Value;

            var business = await _unitOfWork.Repository<Domain.Entities.Business>().GetByIdAsync(request.BusinessId);
            if (business == null)
            {
                _logger.LogWarning("Negocio no encontrado.");
                return Result.Fail(AppErrorFactory.NotFound(nameof(request.BusinessId), request.BusinessId));
            }

            var member = _unitOfWork.Repository<Domain.Entities.Members>()
                .Query()
                .Where(m => m.Id == request.MemberId && m.BusinessId == request.BusinessId && m.Active == Status.Active)
                .FirstOrDefault();

            if (member == null)
            {
                _logger.LogWarning("Miembro no encontrado.");
                return Result.Fail(AppErrorFactory.NotFound(nameof(request.MemberId), request.MemberId));
            }

            var isOwner = business.OwnerId == currentUserId;
            var isSelf = member.UserId == currentUserId;

            // El owner no puede eliminarse a sí mismo
            if (isOwner && isSelf)
            {
                _logger.LogWarning("El owner no puede autoeliminarse.");
                return Result.Fail(AppErrorFactory.Conflict("No puedes eliminar tu propio acceso si eres el dueño del negocio."));
            }

            // Si no soy el owner
            if (!isOwner)
            {
                if (!isSelf)
                {
                    _logger.LogWarning("Usuario {UserId} intentó eliminar a otro miembro sin ser owner.", currentUserId);
                    return Result.Fail(AppErrorFactory.Forbidden("No tienes permisos para eliminar a este miembro."));
                }

                // Es self y no es owner → se permite
            }

            // Se marca como inactivo
            member.Active = Status.Inactive;

            _unitOfWork.Repository<Domain.Entities.Members>().Update(member);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Miembro {MemberId} eliminado (status inactivo)", request.MemberId);
            return Result.Ok();
        }
    }
}