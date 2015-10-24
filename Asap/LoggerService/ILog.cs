using System;
using System.Collections.ObjectModel;
namespace LoggerService
{
    interface ILog
    {
        /// <summary>
        /// Gets and sets all available log messages
        /// </summary>
        ObservableCollection<LogEntry> Logs { get; set; }

        /// <summary>
        /// Add a new log message
        /// </summary>
        void AddLog(LogEntry logEntry);

        /// <summary>
        /// Add the exception
        /// </summary>
        /// <param name="ex">Exception to be logged</param>
        void AddException(Exception ex);
    }
}
