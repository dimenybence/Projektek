using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Models
{
    public class UserRole : IdentityRole
    {
        public UserRole() { }
        public UserRole(string roleName) : base(roleName) { }
    }
}
