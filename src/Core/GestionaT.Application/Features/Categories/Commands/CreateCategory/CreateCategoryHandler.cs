using AutoMapper;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var name = command.Request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Nombre de categoría vacío o nulo.");
                return Result.Fail(AppErrorFactory.BadRequest("El nombre de la categoría es obligatorio."));
            }

            var existing = _unitOfWork.Repository<Category>()
                .QueryIncludingDeleted()
                .FirstOrDefault(c => c.BusinessId == command.BusinessId && c.Name.ToLower() == name.ToLower());

            if (existing is not null)
            {
                if (existing.IsDeleted)
                {
                    _logger.LogInformation("Reactivando categoría eliminada con ID {CategoryId}", existing.Id);
                    existing.IsDeleted = false;
                    _unitOfWork.Repository<Category>().Update(existing);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return Result.Ok(existing.Id);
                }

                _logger.LogWarning("Ya existe una categoría activa con el nombre '{CategoryName}' en el negocio {BusinessId}.", name, command.BusinessId);
                return Result.Fail(AppErrorFactory.Conflict("Ya existe una categoría con ese nombre."));
            }

            var category = _mapper.Map<Category>(command.Request);
            category.BusinessId = command.BusinessId;
            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Categoría creada correctamente con ID {CategoryId}", category.Id);
            return Result.Ok(category.Id);
        }
    }
}