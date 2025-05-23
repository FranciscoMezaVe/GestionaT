using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Commands.UpdateBusinessCommand
{
    public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBusinessCommandHandler> _logger;

        public UpdateBusinessCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateBusinessCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateBusinessCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Domain.Entities.Business>();

            var business = repository.Query()
                .FirstOrDefault(b => b.Id == request.BusinessId);

            if (business is null)
            {
                _logger.LogWarning("No se encontró el negocio con ID: {BusinessId}", request.BusinessId);
                return Result.Fail(new HttpError("Negocio no encontrado", ResultStatusCode.NotFound));
            }

            // Mapear propiedades desde el DTO al entity (actualiza solo Name)
            _mapper.Map(request.Business, business);

            repository.Update(business);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Negocio actualizado: {BusinessId}", business.Id);
            return Result.Ok();
        }
    }
}