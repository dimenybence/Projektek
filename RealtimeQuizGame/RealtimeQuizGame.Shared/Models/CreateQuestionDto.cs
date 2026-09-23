using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models
{
    public record CreateQuestionDto
    {
        [StringLength(2000)]
        public required string Text { get; init; }

        /// <summary>
        /// Kérdés saját idokorlátja másodpercben; null esetén a kvíz alapértelmezett értéke érvényes.
        /// </summary>
        [Range(5, 600)]
        public int? TimeLimitSeconds { get; init; }

        [MinLength(2, ErrorMessage = "Legalább két válaszlehetoség szükséges.")]
        public required IList<CreateAnswerOptionDto> Options { get; init; }                                                                     
    }
}

