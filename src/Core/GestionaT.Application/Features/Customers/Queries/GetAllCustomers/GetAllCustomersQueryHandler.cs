using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<PaginatedList<CustomerResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCustomersQueryHandler> _logger;

        public GetAllCustomersQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllCustomersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<CustomerResponse>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = _unitOfWork.Repository<Customer>()
                .Query()
                .Where(c => c.BusinessId == request.BusinessId);

            if(customers.Count() == 0)
            {
                _logger.LogWarning("No se encontraron clientes para el negocio {BusinessId}", request.BusinessId);
                return Result.Fail(new HttpError("No se encontraron clientes.", ResultStatusCode.NotFound));
            }

            var response = customers.ToPagedList<Customer, CustomerResponse>(_mapper, request.Filters.PageIndex, request.Filters.PageSize);

            _logger.LogInformation("Se obtuvieron {Count} clientes del negocio {BusinessId}", response.Items.Count, request.BusinessId);
            return Result.Ok(response);
        }
    }
}
