using AutoMapper;
using GestionaT.Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Commands.UpdatePatchCategory
{
    public class UpdatePatchCategoryHandler : IRequestHandler<UpdatePatchCategory, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdatePatchCategoryHandler> _logger;
        private readonly IMapper _mapper;

        public UpdatePatchCategoryHandler(IUnitOfWork unitOfWork, ILogger<UpdatePatchCategoryHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<Guid> Handle(UpdatePatchCategory request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Mapeando peticion");
            return Task.FromResult(request.Id);
        }
    }
}
