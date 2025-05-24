using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Members.Queries.GetAllMembersByBusiness
{
    public class GetAllMembersByBusinessQueryHandler : IRequestHandler<GetAllMembersByBusinessQuery, Result<IEnumerable<MembersResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAllMembersByBusinessQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetAllMembersByBusinessQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<GetAllMembersByBusinessQueryHandler> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<MembersResponse>>> Handle(GetAllMembersByBusinessQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            // Validar si el usuario es owner del negocio
            var business = await _unitOfWork.Repository<Domain.Entities.Business>()
                .GetByIdAsync(request.BusinessId);

            if (business == null || business.OwnerId != userId)
            {
                _logger.LogWarning("Usuario {UserId} intentó acceder a miembros del negocio {BusinessId} sin permisos.", userId, request.BusinessId);
                return Result.Fail(new HttpError("No tienes permiso para ver los miembros de este negocio.", ResultStatusCode.Forbidden));
            }

            var ActiveMembers = _unitOfWork.Repository<Domain.Entities.Members>()
                .Query()
                .Where(m => m.BusinessId == request.BusinessId && m.Active == Status.Active)
                .ToList();

            if (!ActiveMembers.Any())
            {
                _logger.LogInformation("No se encontraron miembros activos para el negocio {BusinessId}.", request.BusinessId);
                return Result.Fail(new HttpError("No se encontraron miembros activos.", ResultStatusCode.NotFound));
            }

            var response = _mapper.Map<IEnumerable<MembersResponse>>(ActiveMembers);
            return Result.Ok(response);
        }
    }
}