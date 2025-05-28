using GestionaT.Application.Interfaces.Auth;

namespace GestionaT.Infraestructure.Auth
{
    public class OAuthServiceFactory : IOAuthServiceFactory
    {
        private readonly IEnumerable<IOAuthService> _services;
        public OAuthServiceFactory(IEnumerable<IOAuthService> services)
        {
            _services = services;
        }

        public IOAuthService? GetService(string provider)
        {
            return _services.FirstOrDefault(s => s.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase));
        }

    }
}
