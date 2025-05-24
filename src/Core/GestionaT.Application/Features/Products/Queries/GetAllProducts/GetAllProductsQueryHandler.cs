using AutoMapper;
using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<List<ProductResponse>>>
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

        public async Task<Result<List<ProductResponse>>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
        {
            var products = _unitOfWork.Repository<Product>()
                .QueryIncluding(p => p.Category)
                .Where(p => p.BusinessId == query.BusinessId && !p.IsDeleted)
                .ToList();

            var response = _mapper.Map<List<ProductResponse>>(products);

            _logger.LogInformation("Se obtuvieron {Count} productos del negocio {BusinessId}", response.Count, query.BusinessId);
            return Result.Ok(response);
        }
    }
}
