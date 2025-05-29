using AutoMapper;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCategoryCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Category>();
            var category = repository.Query()
                .FirstOrDefault(c => c.Id == command.Id && c.BusinessId == command.BusinessId);

            if (category is null)
            {
                _logger.LogWarning("Categoría no encontrada: ID {CategoryId}, Business {BusinessId}", command.Id, command.BusinessId);
                return Result.Fail(AppErrorFactory.NotFound(nameof(command.Id), command.Id));
            }

            _mapper.Map(command.Request, category);
            repository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Categoría actualizada correctamente: {CategoryId}", category.Id);
            return Result.Ok();
        }
    }
}
