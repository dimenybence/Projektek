using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.DataAccess.Services;

namespace RealtimeQuizGame.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<RealTimeQuizGameDbContext>(options => options
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies());

            services.AddIdentity<User, UserRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<RealTimeQuizGameDbContext>()
                .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IParticipantTokenService, ParticipantTokenService>();
            services.AddScoped<IQuizzesService, QuizzesService>();

            return services;
        }
    }
}
