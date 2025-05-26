using FluentValidation;
using GestionaT.Application.Features.Products.Commands.AddProductImages;

public class AddProductImagesCommandValidator : AbstractValidator<AddProductImagesCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public AddProductImagesCommandValidator()
    {
        RuleFor(x => x.Images)
            .NotNull().WithMessage("Debes subir al menos una imagen.")
            .Must(images => images!.Count > 0).WithMessage("La lista de imágenes no puede estar vacía.")
            .Must(images => images!.Count <= 5).WithMessage("No puedes subir más de 5 imágenes a la vez.");

        RuleForEach(x => x.Images!).ChildRules(images =>
        {
            images.RuleFor(i => i.FileName)
                .Must(HaveValidExtension).WithMessage("Solo se permiten imágenes con extensiones .jpg, .jpeg, .png o .webp");

            images.RuleFor(i => i.Length)
                .LessThanOrEqualTo(MaxFileSizeInBytes)
                .WithMessage("Cada imagen debe pesar como máximo 5MB.");
        });
    }

    private bool HaveValidExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return _allowedExtensions.Contains(ext);
    }
}
