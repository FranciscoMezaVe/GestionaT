using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedList<ProductResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<ProductResponse>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = _unitOfWork.Repository<Product>()
                .Include(p => p.Category, p => p.Images)
                .Where(p => p.BusinessId == request.BusinessId && !p.IsDeleted);

            if (!products.Any())
            {
                //logging
                _logger.LogWarning("No se encontraron productos, negocio {negocio}", request.BusinessId);
                return Result.Fail(new HttpError($"No se encontraron productos", ResultStatusCode.NotFound));
            }

            var response = products.ToPagedList<Product, ProductResponse>(_mapper, request.PaginationFilters.PageIndex, request.PaginationFilters.PageSize);

            _logger.LogInformation("Se obtuvieron {Count} productos del negocio {BusinessId}", response.Items.Count, request.BusinessId);
            return Result.Ok(response);
        }
    }
}
