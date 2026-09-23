namespace RealtimeQuizGame.Shared.Models;

public record ParticipantOptionDto
{
    public int Id { get; init; }
    public required string Text { get; init; }
}

