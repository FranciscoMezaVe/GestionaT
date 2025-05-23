using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Commands.DeleteBusinessCommand
{
    public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBusinessCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public DeleteBusinessCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteBusinessCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteBusinessCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;
            var repository = _unitOfWork.Repository<Domain.Entities.Business>();

            var business = repository.Query()
                .FirstOrDefault(b => b.Id == request.BusinessId && b.OwnerId == userId);

            if (business is null)
            {
                _logger.LogWarning("Negocio no encontrado o no pertenece al usuario actual");
                return Result.Fail("Negocio no encontrado");
            }

            // Soft delete
            business.IsDeleted = true;

            repository.Update(business);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Negocio {BusinessId} eliminado lógicamente", business.Id);
            return Result.Ok();
        }
    }
}