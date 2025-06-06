﻿using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GestionaT.Application.Features.Business.Commands.UploadBusinessImage
{
    public class UploadBusinessImageCommandHandler : IRequestHandler<UploadBusinessImageCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<UploadBusinessImageCommandHandler> _logger;

        public UploadBusinessImageCommandHandler(IUnitOfWork unitOfWork, ILogger<UploadBusinessImageCommandHandler> logger, IImageStorageService imageStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _imageStorageService = imageStorageService;
        }

        public async Task<Result<string>> Handle(UploadBusinessImageCommand command, CancellationToken cancellationToken)
        {
            var businessRepository = _unitOfWork.Repository<Domain.Entities.Business>();
            var businessImageRepository = _unitOfWork.Repository<BusinessImage>();

            var business = await businessRepository.GetByIdAsync(command.BusinessId);
            if (business is null)
                return Result.Fail(AppErrorFactory.NotFound(nameof(command.BusinessId), command.BusinessId));

            var ImageExists = businessImageRepository.Query()
                .FirstOrDefault(img => img.BusinessId == command.BusinessId);

            if (ImageExists is not null)
            {
                var isDelete = await _imageStorageService.DeleteImageAsync(ImageExists.PublicId);
                if (!isDelete)
                {
                    return Result.Fail(AppErrorFactory.Internal("No se pudo eliminar la imagen anterior."));
                }

                businessImageRepository.Remove(ImageExists);
            }

            var imageUrl = await _imageStorageService.SaveImageAsync(
                command.Image,
                command.Image.FileName,
                businessId: command.BusinessId,
                entity: nameof(business),
                entityId: command.BusinessId
            );

            if(string.IsNullOrEmpty(imageUrl))
            {
                _logger.LogError("Error al guardar la imagen para el negocio {BusinessId}", command.BusinessId);
                return Result.Fail(AppErrorFactory.Internal("No se pudo guardar la imagen."));
            }

            // Extraer publicId si lo necesitas para eliminar luego
            var publicId = _imageStorageService.ExtractPublicIdFromUrl(imageUrl);

            var newImage = new BusinessImage { BusinessId = command.BusinessId, ImageUrl = imageUrl, PublicId = publicId };

            await businessImageRepository.AddAsync(newImage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok(imageUrl);
        }
    }
}
