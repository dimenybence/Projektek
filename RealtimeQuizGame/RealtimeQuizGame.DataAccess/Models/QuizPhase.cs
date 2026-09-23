using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Models
{
    public enum QuizPhase
    {
        Lobby = 0,
        QuestionOpen = 1,
        QuestionResults = 2,
        Completed = 3,
    }
}
