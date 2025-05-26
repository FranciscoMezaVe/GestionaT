using FluentResults;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Users.Queries.GetUserImage
{
    public class GetUserImageQueryHandler : IRequestHandler<GetUserImageQuery, Result<string>>
    {
        private readonly ILogger<GetUserImageQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetUserImageQueryHandler(ILogger<GetUserImageQueryHandler> logger, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public Task<Result<string>> Handle(GetUserImageQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId!.Value;

            var image = _unitOfWork.Repository<UserImage>().Query()
                .FirstOrDefault(i => i.UserId == userId);

            if(image is null)
            {
                return Task.FromResult(Result.Ok(string.Empty));
            }

            return Task.FromResult(Result.Ok(image.ImageUrl));
        }
    }
}
