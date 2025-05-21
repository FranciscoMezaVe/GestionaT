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
        private readonly ICurrentUserService _currentUserService;

        public CreateCategoryHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryHandler> logger, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {

            var exists = await _unitOfWork.Repository<Category>()
                .AnyAsync(c => c.Name == request.Name, cancellationToken);

            if (exists)
            {
                _logger.LogWarning("Ya existe una categoría con el nombre: {CategoryName}", request.Name);
                return Result.Fail<Guid>(new HttpError("Ya existe una categoría con ese nombre.", ResultStatusCode.UnprocesableContent));
            }

            _logger.LogInformation("Mapeando peticion");
            var category = _mapper.Map<Category>(request);

            _logger.LogInformation("Guardando en base de datos");
            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(category.Id);
        }
    }
}
