using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionaT.Application.Interfaces.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestionaT.Infraestructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Guid userId, string userEmail, IList<string> roles, IList<string> bussinessesId)
        {
            string secretKey = Environment.GetEnvironmentVariable("secret-key")!;
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userEmail),
        };
            if(bussinessesId is not null && bussinessesId!.Any())
            {
                claims.Add(new Claim(jwtSettings["Businesses"]!, string.Join(",", bussinessesId!)));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
