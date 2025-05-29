using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Sales.Queries.GetAllSales
{
    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, Result<PaginatedList<SalesResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllSalesQueryHandler> _logger;

        public GetAllSalesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllSalesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Result<PaginatedList<SalesResponse>>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Sale>()
                .Include(x => x.Customer, x => x.Business)
                .Where(x => x.BusinessId == request.BusinessId);

            if (request.Filters.Customer.HasValue)
            {
                query = _unitOfWork.Repository<Sale>()
                .Query()
                .Where(x => x.CustomerId == request.Filters.Customer);
            }

            if (request.Filters.Date.HasValue)
            {
                query = query.Where(x => x.CreatedAt.Date == request.Filters.Date);
            }

            if (request.Filters.From.HasValue && request.Filters.To.HasValue)
            {
                query = query.Where(x => x.CreatedAt > request.Filters.From && x.CreatedAt < request.Filters.To);
            }

            if (!query.Any())
            {
                _logger.LogWarning("No se encontraron ventas para el negocio {BusinessId}", request.BusinessId);
                return Task.FromResult(
                    Result.Fail<PaginatedList<SalesResponse>>(
                        AppErrorFactory.NotFound("Sales", request.BusinessId)));
            }

            var sales = query.ToPagedList<Sale, SalesResponse>(_mapper, request.PaginationFilters.PageIndex, request.PaginationFilters.PageSize);

            return Task.FromResult(Result.Ok(sales));
        }
    }
}
