using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Roles.Queries.GetAllRolesQuery
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<RolesResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllRolesQueryHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<RolesResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = _unitOfWork.Repository<Role>()
                .Query()
                .Where(x => x.BusinessId == request.businessId)
                .ToList();

            if (roles.Count == 0)
            {
                _logger.LogWarning("No se encontraron roles.");
                return Result.Fail(new HttpError("No se encontraron roles.", ResultStatusCode.NoContent));
            }

            var response = _mapper.Map<List<RolesResponse>>(roles);

            _logger.LogInformation("Se encontraron {RolesCount} negocios en la base de datos", response.Count);

            return Result.Ok<IEnumerable<RolesResponse>>(response);
        }
    }
}
