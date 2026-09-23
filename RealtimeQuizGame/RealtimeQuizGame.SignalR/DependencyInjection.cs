using Microsoft.Extensions.DependencyInjection;
using RealtimeQuizGame.SignalR.Services;

namespace RealtimeQuizGame.SignalR;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSingleton<IQuizNotificationService, QuizNotificationService>();
        return services;
    }
}
