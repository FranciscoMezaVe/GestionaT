using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Members.Queries.GetAllMembersByBusiness
{
    public class GetAllMembersByBusinessQueryHandler : IRequestHandler<GetAllMembersByBusinessQuery, Result<PaginatedList<MembersResponse>>>
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

        public async Task<Result<PaginatedList<MembersResponse>>> Handle(GetAllMembersByBusinessQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            var business = await _unitOfWork.Repository<Domain.Entities.Business>()
                .GetByIdAsync(request.BusinessId);

            if (business == null || business.OwnerId != userId)
            {
                _logger.LogWarning("Usuario {UserId} intentó acceder a miembros del negocio {BusinessId} sin permisos.", userId, request.BusinessId);
                return Result.Fail(AppErrorFactory.Forbidden("No tienes permiso para ver los miembros de este negocio."));
            }

            // Traer miembros con Role incluido
            var activeMembers = _unitOfWork.Repository<Domain.Entities.Members>()
                .Include(p => p.Role)
                .Where(m => m.BusinessId == request.BusinessId && m.Active == Status.Active);

            if (!activeMembers.Any())
            {
                _logger.LogInformation("No se encontraron miembros activos para el negocio {BusinessId}.", request.BusinessId);
                return Result.Fail(AppErrorFactory.Conflict("No se encontraron miembros activos."));
            }

            var response = activeMembers.ToPagedList<Domain.Entities.Members, MembersResponse>(_mapper, request.PaginationFilters.PageIndex, request.PaginationFilters.PageSize);

            return Result.Ok(response);
        }
    }
}