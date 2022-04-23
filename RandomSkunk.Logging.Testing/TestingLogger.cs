using Moq;
using System;

namespace Microsoft.Extensions.Logging
{
    public abstract class TestingLogger : ILogger
    {
        private readonly LogLevel _logLevel;

        public TestingLogger() =>
            _logLevel = LogLevel.Trace;

        public TestingLogger(LogLevel logLevel) =>
            _logLevel = logLevel;

        public Mock<IDisposable> BeginScopeReturnValue { get; } = new Mock<IDisposable>();

        public abstract void Log(LogLevel logLevel, EventId eventId, string message, Exception? exception);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Log(logLevel, eventId, formatter(state, exception), exception);

        public virtual bool IsEnabled(LogLevel logLevel) =>
            logLevel >= _logLevel;

        public virtual IDisposable BeginScope<TState>(TState state) =>
            BeginScopeReturnValue.Object;
    }
}
