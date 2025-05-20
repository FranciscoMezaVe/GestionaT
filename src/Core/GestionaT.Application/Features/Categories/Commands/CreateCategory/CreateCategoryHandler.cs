using AutoMapper;
using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategoryHandler> _logger;
        private readonly IMapper _mapper;

        public CreateCategoryHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _unitOfWork.Repository<Category>()
                .AnyAsync(c => c.Name == request.Name, cancellationToken);

                if (exists)
                {
                    _logger.LogWarning("Ya existe una categoría con el nombre: {CategoryName}", request.Name);
                    return Result.Fail<Guid>("Ya existe una categoría con ese nombre.");
                }

                _logger.LogInformation("Mapeando peticion");
                var category = _mapper.Map<Category>(request);

                _logger.LogInformation("Guardando en base de datos");
                await _unitOfWork.Repository<Category>().AddAsync(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok(category.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al crear la categoría: {Message}", e.Message);
                return Result.Fail<Guid>("Error al crear la categoría.");
            }
        }
    }
}
