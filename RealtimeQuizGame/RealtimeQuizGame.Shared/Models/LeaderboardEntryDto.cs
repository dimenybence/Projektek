namespace RealtimeQuizGame.Shared.Models;

public record LeaderboardEntryDto
{
    public int Rank { get; init; }

    public required string DisplayName { get; init; }

    public int CorrectAnswerCount { get; init; }

    public bool IsCurrentParticipant { get; init; }
}
