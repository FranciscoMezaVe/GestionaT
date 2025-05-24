using AutoMapper;
using FluentResults;
using GestionaT.Application.Features.Categories.Queries;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

        public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCategoriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Result<List<CategoryResponse>>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
        {
            var categories = _unitOfWork.Repository<Category>()
                .Query()
                .Where(c => c.BusinessId == query.BusinessId)
                .ToList();

            _logger.LogInformation("Se encontraron {Count} categorías para el negocio {BusinessId}.", categories.Count, query.BusinessId);

            var response = _mapper.Map<List<CategoryResponse>>(categories);
            return Task.FromResult(Result.Ok(response));
        }
    }
}
