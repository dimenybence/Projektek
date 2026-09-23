using Microsoft.AspNetCore.Identity;

namespace RealtimeQuizGame.DataAccess.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = null!;
        public Guid? RefreshToken { get; set; }
    }
}
