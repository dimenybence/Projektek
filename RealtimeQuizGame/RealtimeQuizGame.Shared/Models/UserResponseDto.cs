namespace RealtimeQuizGame.Shared.Models;

/// <summary>
/// Felhasználó adatok válasz DTO.
/// </summary>
public record UserResponseDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}

