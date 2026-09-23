using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butorgyar
{
    public sealed class Logger
    {
        private static Logger instance;
        private List<string> logEntries;

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }

        private Logger()
        {
            logEntries = new List<string>();
        }

        public void Log(string message)
        {
            logEntries.Add(message);
        }

        public List<string> GetLogEntries()
        {
            return logEntries;
        }
    }
}
