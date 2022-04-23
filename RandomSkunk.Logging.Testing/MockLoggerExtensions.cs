using Moq;
using System;
using System.Linq.Expressions;

namespace Microsoft.Extensions.Logging
{
    public static class MockLoggerExtensions
    {
        public static void VerifyLog<TLogger>(
            this Mock<TLogger> mockLogger,
            Action<LogInvocationQuery<TLogger>> configureQuery,
            Times? times = null,
            string? failMessage = null)
            where TLogger : TestingLogger
        {
            var query = new LogInvocationQuery<TLogger>();
            configureQuery(query);
            query.Verify(mockLogger, times, failMessage);
        }

        public static void VerifyLog<TLogger>(
            this Mock<TLogger> mockLogger,
            LogLevel? logLevel = null,
            EventId? eventId = null,
            string? message = null,
            string? messageRegex = null,
            Type? exceptionType = null,
            string? exceptionMessage = null,
            string? exceptionMessageRegex = null,
            Times? times = null,
            string? failMessage = null)
            where TLogger : TestingLogger
        {
            mockLogger.VerifyLog(
                q => q.AtLogLevel(logLevel)
                    .WithEventId(eventId)
                    .WithMessage(message)
                    .WithMessageRegex(messageRegex)
                    .WithException(exceptionType, exceptionMessage, exceptionMessageRegex),
                times,
                failMessage);
        }

        public static void VerifyLog<TLogger>(
            this Mock<TLogger> mockLogger,
            Expression<Func<Exception, bool>> exceptionExpression,
            LogLevel? logLevel = null,
            EventId? eventId = null,
            string? message = null,
            string? messageRegex = null,
            Times? times = null,
            string? failMessage = null)
            where TLogger : TestingLogger
        {
            mockLogger.VerifyLog(
                q => q.AtLogLevel(logLevel)
                    .WithEventId(eventId)
                    .WithMessage(message)
                    .WithMessageRegex(messageRegex)
                    .WithException(exceptionExpression),
                times,
                failMessage);
        }

        public static void VerifyLog<TLogger, TException>(
            this Mock<TLogger> mockLogger,
            Expression<Func<TException, bool>> exceptionExpression,
            LogLevel? logLevel = null,
            EventId? eventId = null,
            string? message = null,
            string? messageRegex = null,
            Times? times = null,
            string? failMessage = null)
            where TLogger : TestingLogger
            where TException : Exception
        {
            mockLogger.VerifyLog(
                q => q.AtLogLevel(logLevel)
                    .WithEventId(eventId)
                    .WithMessage(message)
                    .WithMessageRegex(messageRegex)
                    .WithException(exceptionExpression),
                times,
                failMessage);
        }
    }
}
