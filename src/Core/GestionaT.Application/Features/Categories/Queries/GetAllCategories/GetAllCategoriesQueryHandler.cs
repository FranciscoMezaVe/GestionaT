using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Categories.Queries;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<PaginatedList<CategoryResponse>>>
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

        public Task<Result<PaginatedList<CategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = _unitOfWork.Repository<Category>()
                .Include(c => c.Image)
                .Where(c => c.BusinessId == request.BusinessId);

            if (!categories.Any())
            {
                //logger
                _logger.LogWarning("No se encontraron categorías para el negocio {BusinessId}.", request.BusinessId);
                return Task.FromResult(Result.Fail<PaginatedList<CategoryResponse>>(AppErrorFactory.NotFound(nameof(categories), request.BusinessId)));
            }

            var response = categories.ToPagedList<Category, CategoryResponse>(_mapper, request.Filters.PageIndex, request.Filters.PageSize);

            _logger.LogInformation("Se encontraron {Count} categorías para el negocio {BusinessId}.", response.Items.Count, request.BusinessId);

            
            return Task.FromResult(Result.Ok(response));
        }
    }
}
