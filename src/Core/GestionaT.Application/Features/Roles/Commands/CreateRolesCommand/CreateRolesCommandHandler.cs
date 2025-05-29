using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Roles.Commands.CreateRolesCommand
{
    public class CreateRolesCommandHandler : IRequestHandler<CreateRolesCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateRolesCommandHandler> _logger;

        public CreateRolesCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRolesCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateRolesCommand request, CancellationToken cancellationToken)
        {
            var existing = _unitOfWork.Repository<Role>()
                .QueryIncludingDeleted()
                .FirstOrDefault(r => r.BusinessId == request.BusinessId && r.Name == request.Name);

            if (existing is not null)
            {
                if (existing.IsDeleted)
                {
                    // Reactivar
                    existing.IsDeleted = false;
                    _unitOfWork.Repository<Role>().Update(existing);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result.Ok(existing.Id);
                }

                return Result.Fail(AppErrorFactory.AlreadyExists("Ya existe un rol con ese nombre."));
            }

            var role = _mapper.Map<Role>(request);
            await _unitOfWork.Repository<Role>().AddAsync(role);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Guardando en base de datos");
            return Result.Ok(role.Id);
        }
    }
}
