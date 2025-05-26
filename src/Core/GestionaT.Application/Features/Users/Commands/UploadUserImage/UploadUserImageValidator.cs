using FluentValidation;
using GestionaT.Application.Users.Commands.UploadUserImage;

namespace GestionaT.Application.Features.Users.Commands.UploadUserImage
{
    public class UploadUserImageValidator : AbstractValidator<UploadUserImageCommand>
    {
        public UploadUserImageValidator()
        {
            RuleFor(x => x.Image)
                .NotNull().WithMessage("La imagen es obligatoria.")
                .Must(f => f.Length > 0).WithMessage("La imagen está vacía.")
                .Must(f => new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("Solo se permiten archivos .jpg, .jpeg y .png.");
        }
    }
}