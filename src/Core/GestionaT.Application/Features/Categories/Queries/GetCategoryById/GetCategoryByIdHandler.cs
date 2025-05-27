using AutoMapper;
using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCategoryByIdHandler> _logger;
        private readonly IMapper _mapper;

        public GetCategoryByIdHandler(IUnitOfWork unitOfWork, ILogger<GetCategoryByIdHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Mapeando peticion");

                _logger.LogInformation("Consultando en base de datos");
                var category = _unitOfWork.Repository<Category>()
                    .Include(c => c.Image)
                    .FirstOrDefault(c => c.Id == request.Id);

                if (category is null)
                {
                    _logger.LogInformation("Categoria no encontrada");
                    return Result.Fail(new Error("Categoria no encontrada"));
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var response = _mapper.Map<CategoryResponse>(category);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al consultar la categoria");
                return Result.Fail(new Error("Error al consultar la categoria").CausedBy(e));
            }
        }
    }
}
