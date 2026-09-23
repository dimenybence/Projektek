namespace RealtimeQuizGame.Shared.Models;

/// <summary>
/// Sikeres bejelentkezés válasza (JWT + refresh token).
/// </summary>
public record LoginResponseDto
{
    public required string UserId { get; init; }
    public required string AuthToken { get; init; }
    public required string RefreshToken { get; init; }
}

