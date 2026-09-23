namespace RealtimeQuizGame.DataAccess.Models
{
    public class AnswerOption
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; } = null!;

        public required string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}
