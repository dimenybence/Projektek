using RealtimeQuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Services
{
    public interface IUsersService
    {
        Task AddUserAsync(User user, string password);
        Task<(string authToken, string refreshToken, string userId)> LoginAsync(string email, string password);
        Task<(string authToken, string refreshToken, string userId)> RedeemRefreshTokenAsync(string refreshToken);
        Task LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        string? GetCurrentUserId();
        Task<User> GetUserByIdAsync(string id);
    }
}
