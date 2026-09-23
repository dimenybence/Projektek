using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealtimeQuizGame.DataAccess.Config;
using RealtimeQuizGame.DataAccess.Exeptions;
using RealtimeQuizGame.DataAccess.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealtimeQuizGame.DataAccess.Services
{
    internal class UsersService : IUsersService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<UserRole> _roleManager;

        public UsersService(
            IOptions<JwtSettings> jwtSettings,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<UserRole> roleManager)
        {
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task AddUserAsync(User user, string password)
        {
            user.RefreshToken = Guid.NewGuid();

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidDataException($"Regisztráció sikertelen: {result.Errors.First().Description}");
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new UserRole("User"));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                throw new InvalidDataException($"Szerep hozzárendelése sikertelen: {roleResult.Errors.First().Description}");
            }
        }

        public async Task<(string authToken, string refreshToken, string userId)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new AccessDeniedException("Érvénytelen e-mail vagy jelszó.");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new AccessDeniedException("Túl sok sikertelen próbálkozás — a fiók zárolva.");
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new AccessDeniedException("Érvénytelen e-mail vagy jelszó.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var accessToken = await GenerateJwtTokenAsync(user);

            return (accessToken, user.RefreshToken.ToString()!, user.Id);
        }

        public async Task<(string authToken, string refreshToken, string userId)> RedeemRefreshTokenAsync(string refreshToken)
        {
            if (!Guid.TryParse(refreshToken, out var parsedToken))
            {
                throw new AccessDeniedException("Érvénytelen refresh token.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == parsedToken);
            if (user == null)
            {
                throw new AccessDeniedException("Érvénytelen refresh token.");
            }

            var accessToken = await GenerateJwtTokenAsync(user);

            return (accessToken, refreshToken, user.Id);
        }

        public async Task LogoutAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return;
            }

            await _signInManager.SignOutAsync();
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return null;
            }

            return await _userManager.FindByIdAsync(userId);
        }

        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new AccessDeniedException("A felhasználó nem elérhető.");
            }

            if (user.Id != GetCurrentUserId())
            {
                throw new AccessDeniedException("A felhasználó nem elérhető.");
            }

            return user;
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("id", user.Id),
            new("username", user.UserName!),
        };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
