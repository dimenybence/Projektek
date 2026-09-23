namespace RealtimeQuizGame.Shared.Models;

public record JoinQuizResponseDto
{
    public required string ParticipationToken { get; init; }
    public int QuizId { get; init; }
    public required string QuizTitle { get; init; }
}

