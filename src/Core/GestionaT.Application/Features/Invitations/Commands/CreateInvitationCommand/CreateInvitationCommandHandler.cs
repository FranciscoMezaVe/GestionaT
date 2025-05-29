using AutoMapper;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Invitations.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateInvitationCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateInvitationCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateInvitationCommandHandler> logger,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
        {
            var ownerId = _currentUserService.UserId!.Value;

            // Validar que el current user sea el Owner del negocio
            var business = await _unitOfWork.Repository<Domain.Entities.Business>().GetByIdAsync(request.BusinessId);
            if (business is null || business.OwnerId != ownerId)
            {
                return Result.Fail<Guid>(AppErrorFactory.Forbidden("No tienes permiso para invitar en este negocio."));
            }

            // Validar que el usuario invitado no sea el Owner del negocio
            if (request.Request.InvitedUserId == ownerId)
            {
                return Result.Fail<Guid>(AppErrorFactory.BadRequest("No puedes invitarte a ti mismo."));
            }

            // Validar que el usuario exista
            var invitedUser = await _userRepository.GetByIdAsync(request.Request.InvitedUserId);
            if (invitedUser is null)
            {
                return Result.Fail<Guid>(AppErrorFactory.NotFound(nameof(request.Request.InvitedUserId), request.Request.InvitedUserId));
            }

            // Validar si ya hay una invitación pendiente
            var existingInvitation = await _unitOfWork.Repository<Invitation>()
                .AnyAsync(i => i.BusinessId == request.BusinessId && i.InvitedUserId == request.Request.InvitedUserId && i.Status == InvitationStatus.Pending);

            if (existingInvitation)
            {
                return Result.Fail<Guid>(AppErrorFactory.Conflict("Ya existe una invitación pendiente para este usuario."));
            }

            // Mapear y guardar
            var invitation = _mapper.Map<Invitation>(request.Request);
            invitation.BusinessId = request.BusinessId;
            invitation.Status = InvitationStatus.Pending;

            await _unitOfWork.Repository<Invitation>().AddAsync(invitation);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Invitación enviada a {userId} para el negocio {businessId}", request.Request.InvitedUserId, request.BusinessId);
            return Result.Ok(invitation.Id);
        }
    }
}