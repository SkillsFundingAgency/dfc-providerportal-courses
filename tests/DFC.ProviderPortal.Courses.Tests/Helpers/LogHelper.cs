using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.ProviderPortal.Courses.Tests.Helpers
{
    /// <summary>
    /// Temporary implementation of a wrapper so that ILogger can be passed to code common to both Azure
    /// Functions (which has to use deprecated TraceWriter for logging to work when deployed, ILogger
    /// doesn't work properly yet) and other code such as WebAPI or MVC projects which use ILogger
    /// </summary>
    public class LogHelper : ILogger
    {
        /// <summary>
        /// Private variables holding passed logger object
        /// </summary>
        private TraceWriter _tw = null;
        private ILogger _log = null;

        /// <summary>
        /// Public constructors both require a logger object as an argument
        /// </summary>
        /// <param name="log"></param>
        private LogHelper() { }
        public LogHelper(ILogger log) { _log = log; }
        public LogHelper(TraceWriter tw) { _tw = tw; }

        /// <summary>
        /// Simple wrapper to log event using logger passed to constructor
        /// </summary>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            // Log event
            if (_log != null)                                               // Use ILogger as preferable method
                _log.Log(logLevel, eventId, state, exception, formatter);
            else if (_tw != null)
            {                                         // or TraceWriter as a fallback if available
                switch (logLevel)
                {
                    case LogLevel.Information:
                        _tw.Info(state.ToString()); break;
                    case LogLevel.Warning:
                        _tw.Warning(state.ToString()); break;
                    case LogLevel.Critical:
                    case LogLevel.Error:
                        _tw.Error(state.ToString()); break;
                        //case LogLevel.Trace:
                        //case LogLevel.Debug:
                        //case LogLevel.None:
                        //    break;
                }
            }
        }

        /// <summary>
        /// Required for ILogger implementation - loggers report permanently enabled for now
        /// </summary>
        public bool IsEnabled(LogLevel logLevel) { return true; }

        /// <summary>
        /// Required for ILogger implementation
        /// </summary>
        public IDisposable BeginScope<TState>(TState state) { return null; }
    }
}
