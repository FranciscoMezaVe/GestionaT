using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace GestionaT.Application.Features.Categories.Commands.UploadCategoryImage
{
    public class UploadCategoryImageCommandValidator : AbstractValidator<UploadCategoryImageCommand>
    {
        private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public UploadCategoryImageCommandValidator()
        {
            RuleFor(x => x.Image)
                .NotNull().WithMessage("La imagen es obligatoria.")
                .Must(HaveAllowedExtension).WithMessage("La extensión del archivo no es válida. Se permiten: .jpg, .jpeg, .png, .webp")
                .Must(HaveAllowedContentType).WithMessage("El tipo de contenido no es válido. Se permiten imágenes.")
                .Must(BeWithinFileSizeLimit).WithMessage("El tamaño máximo permitido es de 5 MB.");

            RuleFor(x => x.BusinessId)
                .NotEmpty().WithMessage("El BusinessId no puede estar vacío.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("El CategoryId no puede estar vacío.");
        }

        private bool HaveAllowedExtension(IFormFile file)
        {
            if (file == null) return false;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        private bool HaveAllowedContentType(IFormFile file)
        {
            if (file == null) return false;
            return file.ContentType.StartsWith("image/");
        }

        private bool BeWithinFileSizeLimit(IFormFile file)
        {
            if (file == null) return false;
            return file.Length <= MaxFileSizeInBytes;
        }
    }
}
