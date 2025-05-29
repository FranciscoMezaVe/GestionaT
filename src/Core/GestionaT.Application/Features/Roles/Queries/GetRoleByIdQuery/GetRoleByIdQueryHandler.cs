using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Roles.Queries.GetRoleByIdQuery
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RolesResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRoleByIdQueryHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<RolesResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = _unitOfWork.Repository<Role>()
                .Query()
                .FirstOrDefault(r => r.Id == request.RoleId && r.BusinessId == request.BusinessId);

            if (role == null)
            {
                _logger.LogWarning("No se encontró el rol con ID {RoleId} para el negocio {BusinessId}", request.RoleId, request.BusinessId);
                return Result.Fail(AppErrorFactory.NotFound(nameof(request.RoleId), request.RoleId));
            }

            var response = _mapper.Map<RolesResponse>(role);

            _logger.LogInformation("Rol encontrado: {RoleId}", request.RoleId);
            return Result.Ok(response);
        }
    }
}