using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
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
                .QueryIncluding(p => p.Category)
                .Where(p => p.Id == query.Id && p.BusinessId == query.BusinessId && !p.IsDeleted)
                .FirstOrDefault();

            if (product == null)
            {
                _logger.LogWarning("Producto no encontrado con ID {Id} para el negocio {BusinessId}", query.Id, query.BusinessId);
                return Result.Fail(new HttpError("Producto no encontrado.", ResultStatusCode.NotFound));
            }

            var response = _mapper.Map<ProductResponse>(product);

            return Result.Ok(response);
        }
    }
}
