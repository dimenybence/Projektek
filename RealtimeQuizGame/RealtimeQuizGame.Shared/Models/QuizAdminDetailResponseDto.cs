namespace RealtimeQuizGame.Shared.Models;

public record QuizAdminDetailResponseDto
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string Pin { get; init; }
    public QuizRunStatusDto RunStatus { get; init; }
    public QuizPhaseDto Phase { get; init; }
    public int? CurrentQuestionIndex { get; init; }
    public int DefaultSecondsPerQuestion { get; init; }
    public DateTime? QuestionStartedAtUtc { get; init; }
    public int? RemainingSeconds { get; init; }
    public DateTime ServerUtcNow { get; init; }
    public required IList<QuizQuestionAdminDto> Questions { get; init; }
}

