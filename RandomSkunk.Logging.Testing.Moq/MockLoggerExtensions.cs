using Microsoft.Extensions.Logging;
using Moq.Language.Flow;
using System;

namespace Moq
{
    /// <summary>
    /// Defines extension methods for mock loggers.
    /// </summary>
    public static class MockLoggerExtensions
    {
        /// <summary>
        /// Specifies a default setup on the mock logger for a call to the <see cref="TestingLogger.Log"/> method.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="TestingLogger"/> that is being mocked.</typeparam>
        /// <param name="mockLogger">The mock logger.</param>
        /// <returns>The setup object for the log method.</returns>
        public static ISetup<TLogger> SetupLog<TLogger>(
            this Mock<TLogger> mockLogger)
            where TLogger : TestingLogger
        {
            return mockLogger.Setup(m => m.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<string>(),
                It.IsAny<Exception?>()));
        }

        /// <summary>
        /// Verifies that a specific log invocation was performed on the mock logger.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="TestingLogger"/> that is being mocked.</typeparam>
        /// <param name="mockLogger">The mock logger.</param>
        /// <param name="configureQuery">A function that configures the query that defines the log invocation. If
        ///     <see langword="null"/>, the query is not configured and any log invocation will match.</param>
        /// <param name="times">The number of times a method is expected to be called. If <see langword="null"/>,
        ///     <see cref="Times.AtLeastOnce"/> is used.</param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        public static void VerifyLog<TLogger>(
            this Mock<TLogger> mockLogger,
            Action<ILogInvocationQuery<TLogger>>? configureQuery,
            Times? times = null,
            string? failMessage = null)
            where TLogger : TestingLogger
        {
            if (mockLogger is null)
                throw new ArgumentNullException(nameof(mockLogger));

            var query = new LogInvocationQuery<TLogger>();
            configureQuery?.Invoke(query);
            query.Verify(mockLogger, times, failMessage);
        }

        /// <summary>
        /// Verifies that any log invocations were performed on the mock logger.
        /// </summary>
        /// <typeparam name="TLogger">The type of <see cref="TestingLogger"/> that is being mocked.</typeparam>
        /// <param name="mockLogger">The mock logger.</param>
        /// <param name="times">The number of times a method is expected to be called. If <see langword="null"/>,
        ///     <see cref="Times.AtLeastOnce"/> is used.</param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        public static void VerifyLog<TLogger>(
            this Mock<TLogger> mockLogger,
            Times? times = null,
            string? failMessage = null)
            where TLogger : TestingLogger =>
            mockLogger.VerifyLog(null, times, failMessage);
    }
}
