using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
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

            var exists = await _unitOfWork.Repository<Category>()
                .AnyAsync(c => c.Name == request.Name && c.BusinessId == request.BusinessId, cancellationToken);

            if (exists)
            {
                _logger.LogWarning("Ya existe una categoría con el nombre: {CategoryName}, Negocio: {Business}", request.Name, request.BusinessId);
                return Result.Fail<Guid>(new HttpError("Ya existe una categoría con ese nombre.", ResultStatusCode.UnprocesableContent));
            }

            _logger.LogInformation("Mapeando peticion");
            var category = _mapper.Map<Category>(request);

            _logger.LogInformation("Guardando en base de datos");
            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Categoria guardada con el nombre: {CategoryName}, Negocio: {Business}", request.Name, request.BusinessId);

            return Result.Ok(category.Id);
        }
    }
}
