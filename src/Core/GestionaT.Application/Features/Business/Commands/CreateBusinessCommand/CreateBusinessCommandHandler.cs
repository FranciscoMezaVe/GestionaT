using AutoMapper;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Members.Commands.CreateMembersCommand;
using GestionaT.Application.Features.Roles.Commands.CreateRolesCommand;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Commands.CreateBusinessCommand
{
    public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBusinessCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMediator _mediator;

        public CreateBusinessCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateBusinessCommandHandler> logger, IMapper mapper, ICurrentUserService currentUserService, IBusinessRepository businessRepository, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _businessRepository = businessRepository;
            _mediator = mediator;
        }

        public async Task<Result<Guid>> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            var userId = _currentUserService.UserId!.Value;
            var businessesUserId = _businessRepository.GetAllByUserId(userId);
            if (businessesUserId.Count >= 1)
            {
                _logger.LogWarning("El usuario ya tiene un negocio creado");
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Fail(AppErrorFactory.Conflict("Un usuario no puede tener mas de un negocio propio"));
            }
            //Quiza en futuro validar por el nombre

            var business = _mapper.Map<Domain.Entities.Business>(request);
            business.OwnerId = userId;

            await _unitOfWork.Repository<Domain.Entities.Business>().AddAsync(business);

            var roleResult = await _mediator.Send(new CreateRolesCommand
            {
                Name = RolesValues.Owner,
                BusinessId = business.Id
            }, cancellationToken);

            if (roleResult.IsFailed)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return roleResult;
            }

            var memberResult = await _mediator.Send(new CreateMembersCommand
            {
                BusinessId = business.Id,
                UserId = userId,
                RoleId = roleResult.Value
            }, cancellationToken);

            if (memberResult.IsFailed)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return memberResult;
            }

            await _unitOfWork.CommitTransactionAsync();
            _logger.LogWarning("Guardado en base de datos");
            return Result.Ok(business.Id);
        }
    }
}
