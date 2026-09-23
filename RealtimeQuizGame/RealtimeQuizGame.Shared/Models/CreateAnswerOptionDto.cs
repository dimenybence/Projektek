using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.Shared.Models
{
    public record CreateAnswerOptionDto
    {
        [StringLength(500)]
        public required string Text { get; init; }

        public bool IsCorrect { get; init; }
    }
}

