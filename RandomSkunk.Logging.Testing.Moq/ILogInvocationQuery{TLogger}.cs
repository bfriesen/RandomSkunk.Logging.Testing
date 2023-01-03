using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;

namespace Moq
{
    /// <summary>
    /// Defines a query for verifying the parameters of the <see cref="TestingLogger.Log"/> method.
    /// </summary>
    /// <typeparam name="TLogger">The type of <see cref="TestingLogger"/> to query.</typeparam>
    public interface ILogInvocationQuery<TLogger>
        where TLogger : TestingLogger
    {
        /// <summary>
        /// Verify that the log was made at <see cref="LogLevel.Trace"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtTrace();

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Debug"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtDebug();

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Information"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtInformation();

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Warning"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtWarning();

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtError();

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Critical"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtCritical();

        /// <summary>
        /// Verify that a log was made at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtLogLevel(LogLevel? logLevel);

        /// <summary>
        /// Verify that a log was made at the level specified by the <paramref name="matchLogLevel"/> expression.
        /// </summary>
        /// <param name="matchLogLevel">An expression that defines whether a <see cref="LogLevel"/> is a match for verification.
        ///     </param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> AtLogLevel(Expression<Func<LogLevel, bool>>? matchLogLevel);

        /// <summary>
        /// Verify that a log was made with the specified event ID.
        /// </summary>
        /// <param name="eventId">The expected event ID.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithEventId(EventId? eventId);

        /// <summary>
        /// Verify that a log was made with the event ID specified by the <paramref name="matchEventId"/> expression.
        /// </summary>
        /// <param name="matchEventId">An expression that defines whether an <see cref="EventId"/> is a match for verification.
        ///     </param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithEventId(Expression<Func<EventId, bool>>? matchEventId);

        /// <summary>
        /// Verify that a log was made with the specified message.
        /// </summary>
        /// <param name="message">The expected message.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithMessage(string? message);

        /// <summary>
        /// Verify that a log was made with a message matching the specified regular expression pattern.
        /// </summary>
        /// <param name="messageRegexPattern">The regular expression pattern.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithMessageRegex(string? messageRegexPattern);

        /// <summary>
        /// Verify that a log was made with the message specified by the <paramref name="matchMessage"/> expression.
        /// </summary>
        /// <param name="matchMessage">An expression that defines whether a message is a match for verification.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithMessage(Expression<Func<string, bool>>? matchMessage);

        /// <summary>
        /// Verify that a log was made without an exception.
        /// </summary>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithoutException();

        /// <summary>
        /// Verify that a log was made with an exception.
        /// </summary>
        /// <param name="exceptionType">The type of expected exception.</param>
        /// <param name="message">The message of the expected exception.</param>
        /// <param name="messageRegex">A regular expression pattern that matches the message of the expected exception.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="exceptionType"/> is not assignable to the
        ///     <see cref="Exception"/> type.</exception>
        ILogInvocationQuery<TLogger> WithException(Type? exceptionType = null, string? message = null, string? messageRegex = null);

        /// <summary>
        /// Verify that a log was made with an exception.
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithException<TException>()
            where TException : Exception;

        /// <summary>
        /// Verify that a log was made with an exception with the specified message.
        /// </summary>
        /// <param name="message">The message of the expected exception.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="message"/> is <see langword="null"/>.</exception>
        ILogInvocationQuery<TLogger> WithException(string message);

        /// <summary>
        /// Verify that a log was made with an exception with the specified message.
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <param name="message">The message of the expected exception.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="message"/> is <see langword="null"/>.</exception>
        ILogInvocationQuery<TLogger> WithException<TException>(string message)
            where TException : Exception;

        /// <summary>
        /// Verify that a log was made with an exception specified by the <paramref name="matchException"/> expression.
        /// </summary>
        /// <param name="matchException">An expression that defines whether an exception is a match for verification.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithException(Expression<Func<Exception, bool>>? matchException);

        /// <summary>
        /// Verify that a log was made with an exception specified by the <paramref name="matchException"/> expression.
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <param name="matchException">An expression that defines whether an exception is a match for verification.</param>
        /// <returns>The same instance of <see cref="ILogInvocationQuery{TLogger}"/>.</returns>
        ILogInvocationQuery<TLogger> WithException<TException>(Expression<Func<TException, bool>>? matchException)
            where TException : Exception;
    }
}
