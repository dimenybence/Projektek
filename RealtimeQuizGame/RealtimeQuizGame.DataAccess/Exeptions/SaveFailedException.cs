using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Exeptions
{
    public class SaveFailedException : Exception
    {
        public SaveFailedException()
        {
        }

        public SaveFailedException(string? message)
            : base(message)
        {
        }

        public SaveFailedException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
