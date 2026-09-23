using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Pin { get; set; }

        public required string OwnerId { get; set; }

        public virtual User Owner { get; set; } = null!;

        public QuizRunStatus RunStatus { get; set; }

        public QuizPhase Phase { get; set; }

        public int? CurrentQuestionIndex { get; set; }

        public DateTime? QuestionStartedAtUtc { get; set; }

        public int DefaultSecondsPerQuestion { get; set; } = 30;

        public DateTime CreatedAtUtc { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

        public virtual ICollection<QuizParticipation> Participations { get; set; } = new List<QuizParticipation>();
    }
}
