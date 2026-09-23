using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Services
{
    public interface IParticipantTokenService
    {
        string CreateToken(Guid participationId, int quizId);
    }
}
