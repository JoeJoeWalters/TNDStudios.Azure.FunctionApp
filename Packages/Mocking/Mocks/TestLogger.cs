using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace TNDStudios.Azure.FunctionApp.Mocking.Mocks
{
    /// <summary>
    /// Abstractions made Nullscope internal so it has been redefined here so it is always available 
    /// to the test instances
    /// </summary>
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope() { }

        public void Dispose() { }
    }

    /// <summary>
    /// Test logger used to capture logs to an array for testing later
    /// </summary>
    public class TestLogger : ILogger
    {
        // List of captured logs
        public IList<string> Logs;

        // List of captured exceptions
        public IList<Exception> Exceptions;

        // Scope
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        // Not enabled by default
        public bool IsEnabled(LogLevel logLevel) => false;

        // Default Constructor
        public TestLogger()
        {
            Logs = new List<string>();
            Exceptions = new List<Exception>();
        }

        /// <summary>
        /// Log a given state and set it to the capture array
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            // If it was an exception then log the full detail of the exception
            if (exception != null)
                this.Exceptions.Add(exception);

            // Log the formatted message
            String message = formatter(state, exception);
            this.Logs.Add(message);
        }
    }
}
