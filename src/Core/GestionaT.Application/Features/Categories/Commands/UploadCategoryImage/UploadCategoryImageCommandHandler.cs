using FluentResults;
using GestionaT.Application.Common;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Categories.Commands.UploadCategoryImage
{
    public class UploadCategoryImageCommandHandler : IRequestHandler<UploadCategoryImageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorageService _imageStorageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UploadCategoryImageCommandHandler> _logger;

        public UploadCategoryImageCommandHandler(IUnitOfWork unitOfWork, IImageStorageService imageStorageService, ICurrentUserService currentUserService, ILogger<UploadCategoryImageCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _imageStorageService = imageStorageService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(UploadCategoryImageCommand request, CancellationToken cancellationToken)
        {
            var categoryRepository = _unitOfWork.Repository<Category>();
            var categoryImageRepository = _unitOfWork.Repository<CategoryImage>();

            var category = await categoryRepository.GetByIdAsync(request.CategoryId);
            if (category is null)
                return Result.Fail(new HttpError("Categoria no encontrado.", ResultStatusCode.NotFound));

            var ImageExists = categoryImageRepository.Query()
                .FirstOrDefault(img => img.CategoryId == request.CategoryId);

            if (ImageExists is not null)
            {
                var isDelete = await _imageStorageService.DeleteImageAsync(ImageExists.PublicId);
                if (!isDelete)
                {
                    return Result.Fail(new HttpError("No se pudo eliminar la imagen anterior.", ResultStatusCode.InternalServerError));
                }

                categoryImageRepository.Remove(ImageExists);
            }

            var imageUrl = await _imageStorageService.SaveImageAsync(
                request.Image,
                request.Image.FileName,
                businessId: request.BusinessId,
                entity: nameof(category),
                entityId: request.CategoryId
            );

            // Extraer publicId si lo necesitas para eliminar luego
            var publicId = _imageStorageService.ExtractPublicIdFromUrl(imageUrl);

            var newImage = new CategoryImage { CategoryId = request.CategoryId, ImageUrl = imageUrl, PublicId = publicId };

            await categoryImageRepository.AddAsync(newImage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(imageUrl);
        }
    }
}
