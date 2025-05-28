using GestionaT.Application.Interfaces.Auth;
using GestionaT.Domain.Enums;
using GestionaT.Domain.ValueObjects;
using Newtonsoft.Json;

namespace GestionaT.Infraestructure.Auth.OAuths
{
    public class FacebookAuthService : IOAuthService
    {
        private readonly HttpClient _httpClient;
        public string Provider => OAuthProvidersValues.Facebook;

        public FacebookAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OAuthUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={accessToken}";
            var res = await _httpClient.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("No se pudo conseguir la informacion escencial de facebook");
            }

            var content = await res.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<OAuthUserInfoResult>(content);
            if (data == null)
            {
                throw new Exception("Error al deserializar la respuesta de Facebook.");
            }

            data.Provider = Provider;

            return data;
        }
    }
}
