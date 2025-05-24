namespace GestionaT.Application.Common.Models
{
    public class UserDto
    {
        public Guid Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Nombre { get; set; }
        public bool EmailConfirmado { get; set; }
    }
}
