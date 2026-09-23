namespace RealtimeQuizGame.WebApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }
    }
}
