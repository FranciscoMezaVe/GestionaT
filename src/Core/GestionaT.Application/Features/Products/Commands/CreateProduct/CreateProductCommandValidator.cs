using FluentValidation;
using GestionaT.Application.Features.Products.Commands.CreateProduct;
using Microsoft.AspNetCore.Http;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    private const int _maxImages = 5;
    private const long _maxFileSize = 2 * 1024 * 1024; // 2MB

    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull().WithMessage("Los datos del producto son requeridos.");

        RuleFor(x => x.Request.Images)
            .NotNull().WithMessage("Debes subir al menos una imagen.")
            .Must(images => images.Count > 0).WithMessage("Debes subir al menos una imagen.")
            .Must(images => images.Count <= _maxImages).WithMessage($"Solo puedes subir hasta {_maxImages} imágenes.")
            .ForEach(image =>
            {
                image.Must(BeAnImage).WithMessage("Todos los archivos deben ser imágenes válidas.");
                image.Must(HaveValidExtension).WithMessage($"Extensiones válidas: {string.Join(", ", _allowedExtensions)}");
                image.Must(HaveValidSize).WithMessage($"Cada imagen debe pesar máximo 2MB.");
            });
    }

    private bool BeAnImage(IFormFile file) =>
        file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    private bool HaveValidExtension(IFormFile file) =>
        _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());

    private bool HaveValidSize(IFormFile file) =>
        file.Length <= _maxFileSize;
}
