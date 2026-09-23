using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;

        public required string Text { get; set; }

        public int? TimeLimitSeconds { get; set; }

        public virtual ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();
    }
}
