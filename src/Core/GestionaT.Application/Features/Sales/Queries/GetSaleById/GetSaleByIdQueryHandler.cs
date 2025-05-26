using AutoMapper;
using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Sales.Queries.GetSaleById
{
    public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, Result<SalesResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSaleByIdQueryHandler> _logger;

        public GetSaleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSaleByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Result<SalesResponse>> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            Sale? sale = _unitOfWork.Repository<Sale>()
                .Include(x => x.Customer, x => x.Business)
                .FirstOrDefault(x => x.BusinessId == request.businessId && x.Id == request.SaleId && !x.IsDeleted);

            if (sale == null)
            {
                _logger.LogWarning("Venta con ID {SaleId} no fue econtrada para el negocio {BusinessId}", request.SaleId, request.businessId);
                return Task.FromResult(Result.Fail<SalesResponse>($"Venta con ID {request.SaleId} no econtrada."));
            }

            var response = _mapper.Map<SalesResponse>(sale);
            _logger.LogInformation("Venta con ID {SaleId} fue econtrada para el negocio {BusinessId}", request.SaleId, request.businessId);
            return Task.FromResult(Result.Ok(response));
        }
    }
}
