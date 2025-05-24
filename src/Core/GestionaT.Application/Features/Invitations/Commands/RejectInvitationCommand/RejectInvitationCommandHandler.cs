using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Invitations.Commands.RejectInvitationCommand
{
    public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly ILogger<RejectInvitationCommandHandler> _logger;

        public RejectInvitationCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMediator mediator, ILogger<RejectInvitationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            // Buscar invitación
            var invitation = await _unitOfWork.Repository<Invitation>().GetByIdAsync(request.InvitationId);
            if (invitation == null)
            {
                return Result.Fail(new HttpError("Invitación no encontrada.", ResultStatusCode.NotFound));
            }

            // Validar que el usuario sea el invitado
            if (invitation.InvitedUserId != userId)
            {
                return Result.Fail(new HttpError("No tienes permiso para rechazar esta invitación.", ResultStatusCode.Forbidden));
            }

            // Validar estado pendiente
            if (invitation.Status != InvitationStatus.Pending)
            {
                return Result.Fail(new HttpError("La invitación no está pendiente.", ResultStatusCode.UnprocesableContent));
            }

            // Actualizar estado de invitación
            invitation.Status = InvitationStatus.Rejected;
            _unitOfWork.Repository<Invitation>().Update(invitation);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario {UserId} aceptó invitación {InvitationId} al negocio {BusinessId}.",
                userId, request.InvitationId, invitation.BusinessId);

            return Result.Ok();
        }
    }
}
