using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Moq
{
    /// <summary>
    /// Defines an abstract implementation of the <see cref="ILogger"/> interface meant for
    /// testing.
    /// </summary>
    public abstract class TestingLogger : ILogger
    {
        private readonly List<MockScopeCompletion> _scopeCompletions = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingLogger"/> class.
        /// </summary>
        public TestingLogger() =>
            LogLevel = LogLevel.Trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingLogger"/> class.
        /// </summary>
        /// <param name="logLevel">The log level of the logger.</param>
        public TestingLogger(LogLevel logLevel) =>
            LogLevel = logLevel;

        /// <summary>
        /// Gets the log level of the logger.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Gets the list of mock objects that correspond to calls to <see cref="BeginScope"/>. The
        /// first item in the list corresponds to the first call, the second item to the second
        /// call, and so on.
        /// </summary>
        public IReadOnlyList<MockScopeCompletion> ScopeCompletions => _scopeCompletions;

        /// <summary>
        /// Writes a log entry: called from the <see cref="ILogger.Log{TState}"/> method of this
        /// class.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="message">
        /// Message of the event.<para>When called by the <see cref="Log{TState}"/> method, this is
        /// calculated by calling the <c>formatter</c> parameter and passing the <c>state</c> and
        /// <c>exception</c> parameters as arguments.</para>
        /// </param>
        /// <param name="exception">The exception related to this entry.</param>
        public abstract void Log(LogLevel logLevel, EventId eventId, string message, Exception? exception);

        /// <summary>
        /// Writes a log entry: calls the <see cref="Log"/> method.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">
        /// Function to create a <see cref="string"/> message of the state and exception.
        /// </param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Log(logLevel, eventId, formatter(state, exception), exception);

        /// <summary>
        /// Checks if the given logLevel is enabled: returns whether the
        /// <paramref name="logLevel"/> parameter is greater than or equal to the
        /// <see cref="LogLevel"/> property.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns><see langword="true"/> if enabled.</returns>
        public virtual bool IsEnabled(LogLevel logLevel) =>
            logLevel >= LogLevel;

        /// <summary>
        /// Begins a logical operation scope: adds a new <see cref="MockScopeCompletion{TState}"/>
        /// to the <see cref="ScopeCompletions"/> property and returns its
        /// <see cref="Mock{T}.Object"/> property.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that ends the logical operation scope on dispose.
        /// </returns>
        public virtual IDisposable BeginScope<TState>(TState state)
        {
            var scopeCompletion = new MockScopeCompletion<TState>(state);
            _scopeCompletions.Add(scopeCompletion);
            return scopeCompletion.Object;
        }
    }
}
