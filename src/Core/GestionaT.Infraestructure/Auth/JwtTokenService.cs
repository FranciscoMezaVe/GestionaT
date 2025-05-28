using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestionaT.Infraestructure.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public JwtTokenService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public string GenerateToken(Guid userId, string userEmail, IList<string> roles)
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

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var currentToken = _unitOfWork.Repository<RefreshToken>()
                .Query()
                .FirstOrDefault(x => x.UserId == userId && !x.IsUsed && !x.IsRevoked && x.Expires > DateTime.UtcNow);
            if (currentToken is not null)
            {
                _unitOfWork.Repository<RefreshToken>().Remove(currentToken);
            }
            
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            await _unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            });
            await _unitOfWork.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<bool> RemoveRefreshTokenAsync(Guid userId)
        {
            var refreshTokenRepository = _unitOfWork.Repository<RefreshToken>();
            var currentToken = refreshTokenRepository
                .Query()
                .FirstOrDefault(x => x.UserId == userId && !x.IsUsed && !x.IsRevoked && x.Expires > DateTime.UtcNow);

            if (currentToken is null)
            {
                return false;
            }

            refreshTokenRepository.Remove(currentToken);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
