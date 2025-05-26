using FluentResults;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using GestionaT.Shared.Abstractions;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Common;
using GestionaT.Domain.Enums;

namespace GestionaT.Application.Users.Commands.UploadUserImage
{
    public class UploadUserImageCommandHandler : IRequestHandler<UploadUserImageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorageService _imageStorageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public UploadUserImageCommandHandler(
            IUnitOfWork unitOfWork,
            IImageStorageService imageStorageService,
            ICurrentUserService currentUserService,
            IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _imageStorageService = imageStorageService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<Result<string>> Handle(UploadUserImageCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var userImageRepository = _unitOfWork.Repository<UserImage>();

            if (userId is null)
                return Result.Fail("No estás autenticado.");

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user is null)
                return Result.Fail(new HttpError("Usuario no encontrado.", ResultStatusCode.NotFound));

            var ImageExists = userImageRepository.Query()
                .FirstOrDefault(img => img.UserId == userId.Value);

            if (ImageExists is not null)
            {
                var isDelete = await _imageStorageService.DeleteImageAsync(ImageExists.PublicId);
                if (!isDelete)
                {
                    return Result.Fail(new HttpError("No se pudo eliminar la imagen anterior.", ResultStatusCode.InternalServerError));
                }

                userImageRepository.Remove(ImageExists);
            }

            var imageUrl = await _imageStorageService.SaveImageAsync(
                request.Image,
                request.Image.FileName,
                businessId: userId.Value, // o puedes usar un GUID fijo si no aplica
                entity: "users",
                entityId: userId.Value
            );

            // Extraer publicId si lo necesitas para eliminar luego
            var publicId = _imageStorageService.ExtractPublicIdFromUrl(imageUrl);

            var newImage = new UserImage { UserId = userId.Value, ImageUrl = imageUrl, PublicId = publicId };

            await userImageRepository.AddAsync(newImage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(imageUrl);
        }
    }
}
