using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models;

/// <summary>
/// Bejelentkezési kérés.
/// </summary>
public record LoginRequestDto
{
    /// <summary>
    /// Felhasználó e-mail címe.
    /// </summary>
    [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím.")]
    public required string Email { get; init; }

    /// <summary>
    /// Jelszó.
    /// </summary>
    public required string Password { get; init; }
}

