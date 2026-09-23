using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Models
{
    public class QuizParticipation
    {
        public Guid Id { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;

        public required string DisplayName { get; set; }

        public string? AppUserId { get; set; }

        public virtual User? AppUser { get; set; }

        public DateTime JoinedAtUtc { get; set; }

        public virtual ICollection<QuizParticipantAnswer> Answers { get; set; } = new List<QuizParticipantAnswer>();
    }
}
