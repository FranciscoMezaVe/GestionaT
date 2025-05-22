using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Commands.CreateBusinessCommand
{
    public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCategoryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBusinessRepository _businessRepository;

        public CreateBusinessCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryHandler> logger, IMapper mapper, ICurrentUserService currentUserService, IBusinessRepository businessRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _businessRepository = businessRepository;
        }

        public async Task<Result<Guid>> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;
            var businessesUserId = _businessRepository.GetAllByUserId(userId);
            if (businessesUserId.Count >= 1)
            {
                _logger.LogWarning("El usuario ya tiene un negocio creado");
                return Result.Fail<Guid>(new HttpError("Un usuario no puede tener mas de un negocio propio", ResultStatusCode.UnprocesableContent));
            }
            //Quiza en futuro validar por el nombre

            _logger.LogInformation("Mapeando peticion");
            var business = _mapper.Map<Domain.Entities.Business>(request);
            business.OwnerId = userId;

            _logger.LogInformation("Guardando en base de datos");
            await _unitOfWork.Repository<Domain.Entities.Business>().AddAsync(business);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Guardando en base de datos");
            return Result.Ok(business.Id);
        }
    }
}
