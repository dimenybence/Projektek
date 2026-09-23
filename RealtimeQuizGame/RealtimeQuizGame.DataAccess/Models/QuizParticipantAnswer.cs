using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Models
{
    public class QuizParticipantAnswer
    {
        public int Id { get; set; }

        public Guid ParticipationId { get; set; }

        public virtual QuizParticipation Participation { get; set; } = null!;

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; } = null!;

        public int AnswerOptionId { get; set; }

        public virtual AnswerOption AnswerOption { get; set; } = null!;

        public DateTime SubmittedAtUtc { get; set; }
    }
}
