﻿using AutoMapper;
using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Members.Queries.GetMemberById
{
    public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, Result<MembersResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetMemberByIdQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetMemberByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ILogger<GetMemberByIdQueryHandler> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<MembersResponse>> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            var business = await _unitOfWork.Repository<Domain.Entities.Business>().GetByIdAsync(request.BusinessId);
            if (business == null || business.OwnerId != userId)
            {
                _logger.LogWarning("Usuario {UserId} intentó acceder al miembro {MemberId} sin permisos.", userId, request.MemberId);
                return Result.Fail(AppErrorFactory.Forbidden("No tienes permiso para ver este miembro."));
            }

            var member = _unitOfWork.Repository<Domain.Entities.Members>()
                .Include(m => m.Role)
                .Where(m => m.Id == request.MemberId && m.BusinessId == request.BusinessId && m.Active == Status.Active)
                .FirstOrDefault();

            if (member == null)
            {
                return Result.Fail(AppErrorFactory.NotFound(nameof(request.MemberId), request.MemberId));
            }

            var response = _mapper.Map<MembersResponse>(member);
            return Result.Ok(response);
        }
    }
}