namespace RealtimeQuizGame.Shared.Models;

public record ParticipantQuestionDto
{
    public int Id { get; init; }
    public required string Text { get; init; }
    public required IList<ParticipantOptionDto> Options { get; init; }
}

