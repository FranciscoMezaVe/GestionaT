using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Features.Members.Commands.CreateMembersCommand;
using GestionaT.Application.Features.Roles.Commands.CreateRolesCommand;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Invitations.Commands.AcceptInvitationCommand
{
    public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly ILogger<AcceptInvitationCommandHandler> _logger;

        public AcceptInvitationCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<AcceptInvitationCommandHandler> logger,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            var userId = _currentUserService.UserId!.Value;

            // Buscar invitación
            var invitation = await _unitOfWork.Repository<Invitation>().GetByIdAsync(request.InvitationId);
            if (invitation == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(new HttpError("Invitación no encontrada.", ResultStatusCode.NotFound));
            }

            // Validar que el usuario sea el invitado
            if (invitation.InvitedUserId != userId)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(new HttpError("No tienes permiso para aceptar esta invitación.", ResultStatusCode.Forbidden));
            }

            // Validar estado pendiente
            if (invitation.Status != InvitationStatus.Pending)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(new HttpError("La invitación no está pendiente.", ResultStatusCode.UnprocesableContent));
            }

            // Actualizar estado de invitación
            invitation.Status = InvitationStatus.Accepted;
            _unitOfWork.Repository<Invitation>().Update(invitation);

            var role = _unitOfWork.Repository<Role>()
                .Query()
                .FirstOrDefault(r => r.Name == RolesValues.Worker);

            Guid? roleId = role?.Id;

            if (roleId is null)
            {
                var roleResult = await _mediator.Send(new CreateRolesCommand { Name = RolesValues.Worker, BusinessId = invitation.BusinessId }, cancellationToken);
                if (roleResult.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    var httpError = roleResult.Errors.OfType<HttpError>().FirstOrDefault();
                    return Result.Fail(new HttpError(httpError?.Message ?? "Error al aceptar la invitación.", httpError?.StatusCode ?? ResultStatusCode.UnprocesableContent));
                }

                roleId = roleResult.Value;
            }

            var createMemberCommand = new CreateMembersCommand { BusinessId = invitation.BusinessId, UserId = invitation.InvitedUserId, RoleId = roleId!.Value };

            var memberResult = await _mediator.Send(createMemberCommand, cancellationToken);

            if (memberResult.IsFailed)
            {
                await _unitOfWork.RollbackTransactionAsync();
                var httpError = memberResult.Errors.OfType<HttpError>().FirstOrDefault();
                return Result.Fail(new HttpError(httpError?.Message ?? "Error al aceptar la invitación.", httpError?.StatusCode ?? ResultStatusCode.UnprocesableContent));
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Usuario {UserId} aceptó invitación {InvitationId} al negocio {BusinessId}.",
                userId, request.InvitationId, invitation.BusinessId);

            return Result.Ok();
        }
    }
}
