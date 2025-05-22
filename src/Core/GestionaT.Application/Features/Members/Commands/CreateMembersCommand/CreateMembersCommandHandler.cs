using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Members.Commands.CreateMembersCommand
{
    public class CreateMembersCommandHandler : IRequestHandler<CreateMembersCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateMembersCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CreateMembersCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateMembersCommandHandler> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateMembersCommand request, CancellationToken cancellationToken)
        {
            var exists = _unitOfWork.Repository<Domain.Entities.Members>()
                .Query()
                .Any(x => x.UserId == request.UserId && x.BusinessId == request.BusinessId);

            if (exists)
            {
                _logger.LogWarning("El usuario ya es miembro del negocio");
                return Result.Fail<Guid>(new HttpError("El usuario ya es miembro del negocio", ResultStatusCode.UnprocesableContent));
            }

            _logger.LogInformation("Mapeando peticion");
            var member = _mapper.Map<Domain.Entities.Members>(request);
            await _unitOfWork.Repository<Domain.Entities.Members>().AddAsync(member);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Guardando en base de datos");

            return Result.Ok(member.Id);
        }
    }
}
