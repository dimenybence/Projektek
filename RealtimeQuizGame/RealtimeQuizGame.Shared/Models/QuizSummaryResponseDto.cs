namespace RealtimeQuizGame.Shared.Models;

public record QuizSummaryResponseDto
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string Pin { get; init; }
    public QuizRunStatusDto RunStatus { get; init; }
    public QuizPhaseDto Phase { get; init; }
    public int? CurrentQuestionIndex { get; init; }
    public int QuestionCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

