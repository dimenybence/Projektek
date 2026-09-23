namespace RealtimeQuizGame.SignalR.Services;

public interface IQuizNotificationService
{
    Task NotifyPlayStateUpdatedAsync(int quizId);

    Task NotifyQuizSessionResetAsync(int quizId);
}
