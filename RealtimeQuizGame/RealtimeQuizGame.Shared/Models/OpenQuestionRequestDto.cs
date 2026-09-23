using System.ComponentModel.DataAnnotations;

namespace RealtimeQuizGame.Shared.Models;

public record OpenQuestionRequestDto
{
    [Range(0, int.MaxValue)]
    public int QuestionIndex { get; init; }
}

