using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Queries.GetAllBusinessesQuery
{
    public class GetAllBusinessesQueryHandler : IRequestHandler<GetAllBusinessesQuery, Result<PaginatedList<BusinessReponse>>>
    {
        private readonly ILogger<GetAllBusinessesQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IBusinessRepository _businessRepository;

        public GetAllBusinessesQueryHandler(ILogger<GetAllBusinessesQueryHandler> logger, IMapper mapper, IBusinessRepository businessRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _businessRepository = businessRepository;
        }

        public async Task<Result<PaginatedList<BusinessReponse>>> Handle(GetAllBusinessesQuery request, CancellationToken cancellationToken)
        {
            var businesses = _businessRepository.GetBusinessAccessibleByUser(request.UserId);

            if (businesses.Count == 0)
            {
                _logger.LogWarning("No se encontraron negocios");
                return Result.Fail(new HttpError("No se encontraron negocios", ResultStatusCode.NoContent));
            }

            var response = businesses.AsQueryable().ToPagedList<Domain.Entities.Business, BusinessReponse>(_mapper, request.Filters.PageIndex, request.Filters.PageSize);

            _logger.LogInformation("Se encontraron {BusinessCount} negocios en la base de datos", response.Items.Count);

            return Result.Ok(response);
        }
    }
}
