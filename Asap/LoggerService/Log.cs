using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class Log : ILog
    {
        static readonly Log _instance = new Log();

        #region Properties

        public static Log Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Gets and sets all available log messages
        /// </summary>
        public ObservableCollection<LogEntry> Logs { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor
        /// </summary>
        public Log()
        {
            Logs = new ObservableCollection<LogEntry>();
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Add a new log message
        /// </summary>
        /// <param name="newLog">New log entry</param>
        public void AddLog(LogEntry newLog)
        {
            Logs.Add(newLog);
        }

        /// <summary>
        /// Add the exception
        /// </summary>
        /// <param name="ex">Exception to be logged</param>
        public void AddException(Exception ex)
        {
            Logs.Add(new LogEntry(LogType.Exception, ex.ToString()));
        }

        #endregion
    }
}
