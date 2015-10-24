using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class LogEntry
    {
        public LogType Type { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public LogEntry(LogType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
