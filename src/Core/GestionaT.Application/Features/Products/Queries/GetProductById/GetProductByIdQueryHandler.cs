using AutoMapper;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Product>();

            var product = repository
                .Include(p => p.Category, p => p.Images)
                .Where(p => p.Id == query.Id && p.BusinessId == query.BusinessId && !p.IsDeleted)
                .FirstOrDefault();

            if (product == null)
            {
                _logger.LogWarning("Producto no encontrado con ID {Id} para el negocio {BusinessId}", query.Id, query.BusinessId);
                return Result.Fail(AppErrorFactory.NotFound(nameof(query.Id), query.Id));
            }

            var response = _mapper.Map<ProductResponse>(product);

            return Result.Ok(response);
        }
    }
}
