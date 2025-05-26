using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Roles.Queries.GetAllRolesQuery
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<PaginatedList<RolesResponse>>>
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

        public async Task<Result<PaginatedList<RolesResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = _unitOfWork.Repository<Role>()
                .Query()
                .Where(x => x.BusinessId == request.businessId);

            if (!roles.Any())
            {
                _logger.LogWarning("No se encontraron roles.");
                return Result.Fail(new HttpError("No se encontraron roles.", ResultStatusCode.NoContent));
            }

            var response = roles.ToPagedList<Role, RolesResponse>(_mapper, request.PaginationFilters.PageIndex, request.PaginationFilters.PageSize);

            _logger.LogInformation("Se encontraron {RolesCount} negocios en la base de datos", response.Items.Count);

            return Result.Ok(response);
        }
    }
}
