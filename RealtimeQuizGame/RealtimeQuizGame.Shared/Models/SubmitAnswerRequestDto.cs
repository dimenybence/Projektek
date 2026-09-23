using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models;

public record SubmitAnswerRequestDto
{
    [Range(1, int.MaxValue)]
    public int AnswerOptionId { get; init; }
}

