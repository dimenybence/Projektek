using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models
{
    public record CreateQuizRequestDto
    {
        [StringLength(500)]
        public required string Title { get; init; }

        [Range(5, 600)]
        public int DefaultSecondsPerQuestion { get; init; } = 30;

        [MinLength(1, ErrorMessage = "Legalább egy kérdés szükséges.")]
        public required IList<CreateQuestionDto> Questions { get; init; }
    }
}

