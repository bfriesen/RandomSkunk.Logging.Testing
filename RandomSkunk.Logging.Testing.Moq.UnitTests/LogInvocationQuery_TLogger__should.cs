using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Moq.UnitTests
{
    public class LogInvocationQuery_TLogger__should
    {
        [Fact]
        public void Verify_failure_with_nothing_set()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_trace()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtTrace();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(LogLevel.Trace, It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_debug()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtDebug();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_information()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtInformation();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_warning()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtWarning();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_error()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtError();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_critical()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtCritical();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_at_log_level_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtLogLevel(logLevel => logLevel >= LogLevel.Warning);

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.Is<LogLevel>(logLevel => (int)logLevel >= 3), It.IsAny<EventId>(), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_event_id()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithEventId(123);

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), 123, It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_event_id_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithEventId(eventId => eventId.Id == 123 && eventId.Name == "MyEventId");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.Is<EventId>(eventId => eventId.Id == 123 && eventId.Name == \"MyEventId\"), It.IsAny<string>(), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_message()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithMessage("My message");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), \"My message\", It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_message_regex()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithMessageRegex(@"^My message(?:\(s\))?$");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage(@"*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsRegex(""^My message(?:\(s\))?$""), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_message_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithMessage(message => message.Contains("My message"));

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.Is<string>(message => message.Contains(\"My message\")), It.IsAny<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_without_exception()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithoutException();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), null)*");
        }

        [Fact]
        public void Verify_failure_with_with_exception()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.IsNotNull<Exception>())*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_type_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(typeof(InvalidOperationException));

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.IsNotNull<InvalidOperationException>())*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_message_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException("My exception message");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<Exception>(ex => ex != null && ex.Message == \"My exception message\"))*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_message_regex_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(messageRegex: "[abc][123]");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage(@"*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<Exception>(ex => ex != null && Regex.IsMatch(ex.Message, ""[abc][123]"")))*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_type_and_message_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(typeof(InvalidOperationException), "My exception message");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<InvalidOperationException>(ex => ex != null && ex.Message == \"My exception message\"))*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_type_and_message_regex_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(typeof(InvalidOperationException), messageRegex: "[abc][123]");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage(@"*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<InvalidOperationException>(ex => ex != null && Regex.IsMatch(ex.Message, ""[abc][123]"")))*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_type_specified_generically()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException<InvalidOperationException>();

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.IsNotNull<InvalidOperationException>())*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_message_and_type_specified_generically()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException<InvalidOperationException>("My exception message");

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<InvalidOperationException>(ex => ex != null && ex.Message == \"My exception message\"))*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(ex => ex != null && ex.Message.Contains("abc"));

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<Exception>(ex => ex != null && ex.Message.Contains(\"abc\")))*");
        }

        [Fact]
        public void Verify_failure_with_with_exception_generic_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException<InvalidOperationException>(ex => ex != null && ex.Message.Contains("abc"));

            var mockLogger = new MockLogger();

            Action act = () => logInvocationQuery.Verify(mockLogger);

            act.Should().ThrowExactly<MockException>()
                .WithMessage("*logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<string>(), It.Is<InvalidOperationException>(ex => ex != null && ex.Message.Contains(\"abc\")))*");
        }

        [Fact]
        public void Verify_success_with_nothing_set()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_trace()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtTrace();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogTrace("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_debug()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtDebug();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogDebug("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_information()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtInformation();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_warning()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtWarning();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogWarning("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_error()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtError();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogError("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_critical()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtCritical();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogCritical("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_at_log_level_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .AtLogLevel(logLevel => logLevel >= LogLevel.Warning);

            var mockLogger = new MockLogger();
            mockLogger.Object.LogCritical("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_event_id()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithEventId(123);

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(123, "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_event_id_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithEventId(eventId => eventId.Id == 123 && eventId.Name == "MyEventId");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new EventId(123, "MyEventId"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_message()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithMessage("My message");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_message_regex()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithMessageRegex(@"^My message(?:\(s\))?$");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation("My message(s)");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_message_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithMessage(message => message.Contains("My message"));

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_without_exception()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithoutException();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation("My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException(), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_type_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(typeof(InvalidOperationException));

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException(), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_message_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException("My exception message");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_message_regex_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(messageRegex: "[abc][123]");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My a1 exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_type_and_message_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(typeof(InvalidOperationException), "My exception message");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_type_and_message_regex_specified()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(typeof(InvalidOperationException), messageRegex: "[abc][123]");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My a1 exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_type_specified_generically()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException<InvalidOperationException>();

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException(), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_message_and_type_specified_generically()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException<InvalidOperationException>("My exception message");

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException(ex => ex != null && ex.Message.Contains("abc"));

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My abc exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }

        [Fact]
        public void Verify_success_with_with_exception_generic_expression()
        {
            var logInvocationQuery = new LogInvocationQuery<TestingLogger>()
                .WithException<InvalidOperationException>(ex => ex != null && ex.Message.Contains("abc"));

            var mockLogger = new MockLogger();
            mockLogger.Object.LogInformation(new InvalidOperationException("My abc exception message"), "My message");

            logInvocationQuery.Verify(mockLogger);
        }
    }
}
