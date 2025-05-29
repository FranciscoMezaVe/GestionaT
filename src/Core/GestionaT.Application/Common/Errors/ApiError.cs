namespace GestionaT.Application.Common.Errors
{
    public class ApiError
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? Detail { get; set; }
        public string? Target { get; set; }
    }

}
