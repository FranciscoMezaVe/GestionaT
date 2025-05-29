using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Category>();

            var category = repository
                .Query()
                .FirstOrDefault(c => c.Id == command.Id && c.BusinessId == command.BusinessId && !c.IsDeleted);

            if (category is null)
            {
                _logger.LogWarning("Categoría no encontrada: ID {CategoryId}, BusinessId {BusinessId}", command.Id, command.BusinessId);
                return Result.Fail(AppErrorFactory.NotFound(nameof(command.Id), command.Id));
            }

            if (category.Products.Any())
            {
                _logger.LogWarning("La categoría {CategoryId} no puede ser eliminada porque tiene productos asignados.", category.Id);
                return Result.Fail(AppErrorFactory.Conflict("No se puede eliminar la categoría porque tiene productos asignados."));
            }

            repository.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Categoría eliminada correctamente: ID {CategoryId}", category.Id);
            return Result.Ok();
        }
    }
}