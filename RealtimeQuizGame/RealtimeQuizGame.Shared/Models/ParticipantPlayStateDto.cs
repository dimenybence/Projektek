namespace RealtimeQuizGame.Shared.Models;

public record ParticipantPlayStateDto
{
    public int QuizId { get; init; }
    public required string QuizTitle { get; init; }
    public QuizRunStatusDto RunStatus { get; init; }
    public QuizPhaseDto Phase { get; init; }
    public int? CurrentQuestionIndex { get; init; }
    public int? RemainingSeconds { get; init; }
    public DateTime ServerUtcNow { get; init; }
    public ParticipantQuestionDto? CurrentQuestion { get; init; }
    public IList<QuestionResultOptionDto>? ResultBreakdown { get; init; }
    public int? SelectedAnswerOptionId { get; init; }
    public bool HasAnsweredCurrentQuestion { get; init; }

    public IList<LeaderboardEntryDto>? Leaderboard { get; init; }
}

