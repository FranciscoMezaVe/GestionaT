﻿using AutoMapper;
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

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInivitationsByUser
{
    public class GetAllInvitationByUserQueryHandler : IRequestHandler<GetAllInvitationByUserQuery, Result<IEnumerable<InvitationResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllInvitationByUserQueryHandler> _logger;

        public GetAllInvitationByUserQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, ILogger<GetAllInvitationByUserQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<Result<IEnumerable<InvitationResponse>>> Handle(GetAllInvitationByUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            var query = _unitOfWork.Repository<Invitation>()
                .Query()
                .Where(i => i.InvitedUserId == userId);

            if (request.Filters.Status.HasValue)
            {
                query = query.Where(i => i.Status == request.Filters.Status);
            }

            if (request.Filters.Business.HasValue)
            {
                query = query.Where(i => i.BusinessId == request.Filters.Business);
            }

            var invitations = query.OrderByDescending(i => i.CreatedAt)
                .ProjectTo<InvitationResponse>(_mapper.ConfigurationProvider)
                .AsEnumerable();

            if (!invitations.Any())
            {
                _logger.LogInformation("No se econtraron invitaciones para el usuario {userId}", userId);
                return Task.FromResult(Result.Fail<IEnumerable<InvitationResponse>>(new HttpError("No se encontraron invitaciones.", ResultStatusCode.NotFound)));
            }

            return Task.FromResult(Result.Ok(invitations));
        }
    }
}
