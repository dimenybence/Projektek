namespace RealtimeQuizGame.Shared.SignalR.HubInterfaces;

public interface IQuizClient
{
    Task PlayStateUpdated(int quizId);

    Task QuizSessionReset(int quizId);
}
