using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Moq.UnitTests
{
    public class TestingLogger_should
    {
        [Fact]
        public void Set_LogLevel_to_trace_in_default_constructor()
        {
            TestingLogger logger = new ConcreteTestingLogger();

            logger.LogLevel.Should().Be(LogLevel.Trace);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        public void Set_LogLevel_in_constructor(LogLevel logLevel)
        {
            TestingLogger logger = new ConcreteTestingLogger(logLevel);

            logger.LogLevel.Should().Be(logLevel);
        }

        [Fact]
        public void Return_IsEnabled_based_on_LogLevel()
        {
            TestingLogger logger = new ConcreteTestingLogger(LogLevel.Warning);

            logger.IsEnabled(LogLevel.Trace).Should().BeFalse();
            logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
            logger.IsEnabled(LogLevel.Information).Should().BeFalse();
            logger.IsEnabled(LogLevel.Warning).Should().BeTrue();
            logger.IsEnabled(LogLevel.Error).Should().BeTrue();
            logger.IsEnabled(LogLevel.Critical).Should().BeTrue();
        }

        [Fact]
        public void Return_MockDisposable_object_when_calling_BeginScope()
        {
            var logger = new ConcreteTestingLogger();

            var disposable = logger.BeginScope("My message: {body}", 123);

            logger.ScopeCompletions.Should().HaveCount(1);
            disposable.Should().BeSameAs(logger.ScopeCompletions[0].Object);
        }

        [Fact]
        public void Logs_to_abstract_Log_method()
        {
            var mockLogger = new Mock<TestingLogger>();

            var state = new MyStateObject();
            var exception = new InvalidOperationException("My exception message");
            MyStateObject? capturedState = null;
            Exception? capturedException = null;

            string Formatter(MyStateObject state, Exception? ex)
            {
                capturedState = state;
                capturedException = ex;
                return "My formatted message";
            }

            mockLogger.Object.Log(LogLevel.Information, 123, state, exception, Formatter);

            mockLogger.Verify(m => m.Log(LogLevel.Information, 123, "My formatted message", exception));

            capturedState.Should().BeSameAs(state);
            capturedException.Should().BeSameAs(exception);
        }

        private class MyStateObject
        {
        }

        private class ConcreteTestingLogger : TestingLogger
        {
            public ConcreteTestingLogger()
            {
            }

            public ConcreteTestingLogger(LogLevel logLevel)
                : base(logLevel)
            {
            }

            public override void Log(LogLevel logLevel, EventId eventId, string message, Exception? exception)
            {
            }
        }
    }
}
