namespace RealtimeQuizGame.Shared.Models;

public record QuestionResultOptionDto
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public int AnswerCount { get; init; }
    public bool IsCorrect { get; init; }
}

