using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models;

/// <summary>
/// Regisztrációs kérés.
/// </summary>
public record UserRequestDto
{
    [StringLength(255, ErrorMessage = "A név túl hosszú.")]
    public required string Name { get; init; }

    [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím.")]
    public required string Email { get; init; }

    public required string Password { get; init; }
}

