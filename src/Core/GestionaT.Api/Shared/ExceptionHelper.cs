namespace GestionaT.Api.Shared
{
    public static class ExceptionHelper
    {
        public static string? GetDetail(Exception ex, IServiceProvider services)
        {
            var env = services.GetRequiredService<IHostEnvironment>();
            return env.IsDevelopment() ? ex.ToString() : null;
        }
    }
}
