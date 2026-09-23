using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealtimeQuizGame.DataAccess.Config;
using RealtimeQuizGame.DataAccess.Services;
using RealtimeQuizGame.Shared.SignalR.HubInterfaces;
using RealtimeQuizGame.SignalR.Services;

namespace RealtimeQuizGame.SignalR.Hubs;

[Authorize(AuthenticationSchemes = $"Bearer,{AuthSchemes.ParticipantJwt}")]
public class QuizHub : Hub<IQuizClient>
{
    private readonly IQuizzesService _quizzesService;

    public QuizHub(IQuizzesService quizzesService)
    {
        _quizzesService = quizzesService;
    }

    public async Task JoinQuizGroup(int quizId)
    {
        await EnsureCanJoinQuizGroupAsync(quizId);
        await Groups.AddToGroupAsync(Context.ConnectionId, QuizNotificationService.GetGroupName(quizId));
    }

    public async Task LeaveQuizGroup(int quizId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, QuizNotificationService.GetGroupName(quizId));
    }

    private async Task EnsureCanJoinQuizGroupAsync(int quizId)
    {
        var claimQuizId = GetQuizIdFromClaims();
        if (claimQuizId != null)
        {
            if (claimQuizId.Value != quizId)
            {
                throw new HubException("A részvételi token nem ehhez a kvízhez tartozik.");
            }

            return;
        }

        var ownerId = Context.User?.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(ownerId))
        {
            throw new HubException("Nincs jogosultság ehhez a kvízhez.");
        }

        await _quizzesService.GetOwnedQuizAsync(quizId, ownerId);
    }

    private int? GetQuizIdFromClaims()
    {
        var raw = Context.User?.FindFirst(AuthClaimTypes.QuizId)?.Value;
        return int.TryParse(raw, out var quizId) ? quizId : null;
    }
}
