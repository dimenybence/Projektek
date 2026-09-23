using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Exeptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string name)
            : base($"{name} entitás nem található.")
        {
        }
    }
}
