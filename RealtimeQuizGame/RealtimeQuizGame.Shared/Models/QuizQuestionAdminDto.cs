namespace RealtimeQuizGame.Shared.Models;

public record QuizQuestionAdminDto
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public int? TimeLimitSeconds { get; init; }
    public required IList<QuizAnswerOptionAdminDto> Options { get; init; }
}

