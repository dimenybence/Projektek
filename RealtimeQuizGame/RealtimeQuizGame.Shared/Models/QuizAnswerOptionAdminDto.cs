namespace RealtimeQuizGame.Shared.Models;

public record QuizAnswerOptionAdminDto
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public bool IsCorrect { get; init; }
}

