using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

public class GetAllInvitationsQueryHandler : IRequestHandler<GetAllInvitationsQuery, Result<IEnumerable<InvitationResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllInvitationsQueryHandler> _logger;

    public GetAllInvitationsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetAllInvitationsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<Result<IEnumerable<InvitationResponse>>> Handle(GetAllInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var invitations = _unitOfWork.Repository<Invitation>()
            .QueryIncluding(i => i.Business)
            .Where(i => i.BusinessId == request.businessId)
            .OrderByDescending(i => i.CreatedAt)
            .ProjectTo<InvitationResponse>(_mapper.ConfigurationProvider)
            .AsEnumerable();

        if (!invitations.Any())
        {
            _logger.LogInformation("No se econtraron invitaciones, negocio: {businessId}", request.businessId);
            return Task.FromResult(Result.Fail<IEnumerable<InvitationResponse>>(new HttpError("No se encontraron invitaciones.", ResultStatusCode.NotFound)));
        }
        
        return Task.FromResult(Result.Ok(invitations));
    }
}