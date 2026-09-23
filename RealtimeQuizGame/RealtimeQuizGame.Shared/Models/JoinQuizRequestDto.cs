using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models;

public record JoinQuizRequestDto
{
    [StringLength(16, MinimumLength = 4)]
    public required string Pin { get; init; }

    [StringLength(40, MinimumLength = 1)]
    public required string DisplayName { get; init; }
}

