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

            var memberExists = _unitOfWork.Repository<Domain.Entities.Members>()
                    .Query()
                    .FirstOrDefault(x => x.UserId == request.UserId && x.BusinessId == request.BusinessId);

            if (memberExists is not null)
            {
                if (memberExists.Active == Status.Active)
                {
                    _logger.LogWarning("El usuario ya es miembro activo del negocio");
                    return Result.Fail<Guid>(new HttpError("El usuario ya es miembro del negocio", ResultStatusCode.UnprocesableContent));
                }

                _logger.LogInformation("Reactivando miembro inactivo");
                var result = await ReactivateMember(memberExists, cancellationToken);
                return result;
            }

            _logger.LogInformation("Mapeando nuevo miembro desde la petición");
            var member = _mapper.Map<Domain.Entities.Members>(request);
            member.IsAccepted = true;

            await _unitOfWork.Repository<Domain.Entities.Members>().AddAsync(member);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Nuevo miembro guardado en base de datos");
            return Result.Ok(member.Id);
        }

        private async Task<Result<Guid>> ReactivateMember(Domain.Entities.Members member, CancellationToken cancellationToken)
        {
            member.Active = Status.Active;
            _unitOfWork.Repository<Domain.Entities.Members>().Update(member);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Miembro reactivado en base de datos");
            return Result.Ok(member.Id);
        }
    }
}
