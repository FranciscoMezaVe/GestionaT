using GestionaT.Application.Interfaces.Auth;
using GestionaT.Persistence.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GestionaT.Infraestructure.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ILogger<AuthenticationService> _logger;

        public AuthenticationService(UserManager<ApplicationUser> userManager, ILogger<AuthenticationService> logger, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("No se encontró el usuario con email {email}", email);
                return false;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _logger.LogInformation("Usuario {email} no se autentico correctamente", email);
                return false;
            }

            _logger.LogInformation("Usuario {email} se autentico correctamente", email);
            return true;
        }

        public async Task<bool> IsExistsUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<bool> IsExistsUserByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString()) is not null;
        }

        public async Task<Guid> GetUserIdAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Usuario con el correo {Email} no se econtro", email);
                return Guid.Empty;
            }

            _logger.LogInformation("Usuario con el correo {Email} se econtro con id {id}", email, user.Id);
            return user.Id;
        }
        public async Task<IList<string>?> GetBusinessesIdAsync(Guid userId)
        {
            var user = await _userManager.Users
                .Include(u => u.MemberBusinesses)
                .Include(u => u.OwnedBusinesses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning("Usuario con el id {id} no se encontró", userId);
                return null;
            }

            var memberBusinessIds = user.MemberBusinesses?
                .Select(ub => ub.BusinessId.ToString()) ?? new List<string>();

            var ownedBusinessIds = user.OwnedBusinesses?
                .Select(b => b.Id.ToString()) ?? new List<string>();

            var allBusinessIds = memberBusinessIds
                .Union(ownedBusinessIds)
                .Distinct()
                .ToList();

            if (!allBusinessIds.Any())
            {
                _logger.LogWarning("Usuario con el id {id} no tiene negocios como miembro ni propietario", userId);
                return null;
            }

            return allBusinessIds;
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Usuario con el id {id} no se econtro", userId);
                return new List<string>();
            }
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public Task<bool> RegisterUserAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfirmEmailAsync(string email, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserEmailAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user.Email!;
        }
    }
}
