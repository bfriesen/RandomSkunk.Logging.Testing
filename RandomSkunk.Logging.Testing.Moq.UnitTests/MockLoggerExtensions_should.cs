using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Moq.UnitTests
{
    public class MockLoggerExtensions_should
    {
        [Fact]
        public void Verify_failure()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.LogInformation("Hello, world!");

            mockLogger.Invoking(log => log.VerifyLog(Times.Never()))
                .Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_success()
        {
            var mockLogger = new MockLogger();

            mockLogger.Object.LogInformation("Hello, world!");

            mockLogger.VerifyLog(log => log.AtInformation().WithMessage("Hello, world!"));
        }

        [Fact]
        public void Setup_correctly()
        {
            var mockLogger = new MockLogger();

            LogLevel capturedLogLevel = LogLevel.None;
            EventId capturedEventId = -1;
            string? capturedMessage = null;
            Exception? capturedException = null;

            mockLogger.SetupLog()
                .Callback<LogLevel, EventId, string, Exception>(
                    (logLevel, eventId, message, exception) =>
                    {
                        capturedLogLevel = logLevel;
                        capturedEventId = eventId;
                        capturedMessage = message;
                        capturedException = exception;
                    });

            LogLevel expectedLogLevel = LogLevel.Information;
            EventId expectedEventId = 123;
            const string? expectedMessage = "Hello, world!";
            Exception? expectedException = new();

            mockLogger.Object.LogInformation(expectedEventId, expectedException, expectedMessage);

            capturedLogLevel.Should().Be(expectedLogLevel);
            capturedEventId.Should().Be(expectedEventId);
            capturedMessage.Should().Be(expectedMessage);
            capturedException.Should().Be(expectedException);
        }
    }
}
