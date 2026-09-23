using Microsoft.AspNetCore.SignalR;
using RealtimeQuizGame.Shared.SignalR.HubInterfaces;
using RealtimeQuizGame.SignalR.Hubs;

namespace RealtimeQuizGame.SignalR.Services;

internal class QuizNotificationService : IQuizNotificationService
{
    private readonly IHubContext<QuizHub, IQuizClient> _hubContext;

    public QuizNotificationService(IHubContext<QuizHub, IQuizClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyPlayStateUpdatedAsync(int quizId)
    {
        return _hubContext.Clients.Group(GetGroupName(quizId)).PlayStateUpdated(quizId);
    }

    public Task NotifyQuizSessionResetAsync(int quizId)
    {
        return _hubContext.Clients.Group(GetGroupName(quizId)).QuizSessionReset(quizId);
    }

    internal static string GetGroupName(int quizId) => quizId.ToString();
}
