using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
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
            var exists = _unitOfWork.Repository<Role>().Query()
                .Any(x => x.Name == request.Name && x.BusinessId == request.BusinessId);

            if (exists)
            {
                _logger.LogWarning("Ya existe un rol con nombre {name} en el negocio {business}", request.Name, request.BusinessId);
                return Result.Fail<Guid>(new HttpError("Ya existe un rol con ese nombre", ResultStatusCode.UnprocesableContent));    
            }

            var role = _mapper.Map<Role>(request);
            await _unitOfWork.Repository<Role>().AddAsync(role);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Guardando en base de datos");
            return Result.Ok(role.Id);
        }
    }
}
