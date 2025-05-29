using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Reports;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Sales.Queries.GetReportSales
{
    class GetReportSalesQueryHandler : IRequestHandler<GetReportSalesQuery, Result<byte[]>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportService _reportService;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<GetReportSalesQueryHandler> _logger;

        public GetReportSalesQueryHandler(IUnitOfWork unitOfWork, IReportService reportService, ILogger<GetReportSalesQueryHandler> logger, ISaleRepository saleRepository)
        {
            _unitOfWork = unitOfWork;
            _reportService = reportService;
            _logger = logger;
            _saleRepository = saleRepository;
        }

        public async Task<Result<byte[]>> Handle(GetReportSalesQuery request, CancellationToken cancellationToken)
        {
            var query = _saleRepository.GetSalesForReport();

            if (request.Filters.Customer.HasValue)
            {
                query = query.Where(x => x.CustomerId == request.Filters.Customer);
            }

            if (request.Filters.Date.HasValue)
            {
                query = query.Where(x => x.CreatedAt.Date == request.Filters.Date);
            }

            if (request.Filters.From.HasValue && request.Filters.To.HasValue)
            {
                query = query.Where(x => x.CreatedAt > request.Filters.From && x.CreatedAt < request.Filters.To);
            }

            var sales = query.OrderByDescending(i => i.CreatedAt)
                .ToList();

            if (sales.Count == 0)
            {
                _logger.LogWarning("No se encontraron ventas para el negocio {BusinessId}", request.BusinessId);
                return Result.Fail(
                        AppErrorFactory.NotFound("Sales", request.BusinessId));
            }

            var report = await _reportService.GenerateSaleReportPdfAsync(sales);

            return Result.Ok(report);
        }
    }
}
