using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.Shared.Models;

namespace RealtimeQuizGame.DataAccess.Services
{
    public interface IQuizzesService
    {
        Task<Quiz> CreateQuizAsync(string ownerId, CreateQuizRequestDto request);
        Task<Quiz> UpdateQuizAsync(int quizId, string ownerId, CreateQuizRequestDto request);
        Task<PaginatedResult<Quiz>> ListMineAsync(string ownerId, int page, int pageSize);
        Task<PaginatedResult<Quiz>> ListJoinableAsync(int page, int pageSize);
        Task<Quiz> GetOwnedQuizAsync(int quizId, string ownerId);
        Task<bool> StartQuizAsync(int quizId, string ownerId);
        Task OpenQuestionAsync(int quizId, string ownerId, int questionIndex);
        Task CloseQuestionAsync(int quizId, string ownerId);
        Task AdvanceAfterResultsAsync(int quizId, string ownerId);
        Task<(QuizParticipation participation, string token)> JoinByPinAsync(string pin, string displayName, string? appUserId);
        Task<ParticipantPlayStateDto> GetParticipantPlayStateAsync(Guid participationId);
        Task SubmitAnswerAsync(Guid participationId, int answerOptionId);
    }
}
