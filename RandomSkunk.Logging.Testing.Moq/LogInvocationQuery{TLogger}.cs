using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Moq
{
    /// <summary>
    /// Defines a query for verifying the parameters of the <see cref="TestingLogger.Log"/> method.
    /// </summary>
    /// <typeparam name="TLogger">The type of <see cref="TestingLogger"/> to query.</typeparam>
    public class LogInvocationQuery<TLogger>
        where TLogger : TestingLogger
    {
        // Methods used when manually constructing lambda expressions.
        private static readonly MethodInfo _itIsAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), Type.EmptyTypes) ?? throw new InvalidOperationException("Cannot find It.IsAny method.");
        private static readonly MethodInfo _itIsRegexMethod = typeof(It).GetMethod(nameof(It.IsRegex), new[] { typeof(string) }) ?? throw new InvalidOperationException("Cannot find It.IsRegex method.");
        private static readonly MethodInfo _itIsMethod = typeof(It).GetMethods().SingleOrDefault(HasExpressionOfFuncOfTValueToBoolParameter) ?? throw new InvalidOperationException("Cannot find It.Is method.");
        private static readonly MethodInfo _itIsNotNullMethod = typeof(It).GetMethod(nameof(It.IsNotNull), Type.EmptyTypes) ?? throw new InvalidOperationException("Cannot find It.IsNotNull method.");
        private static readonly MethodInfo _testingLoggerlogMethod = typeof(TestingLogger).GetMethod(nameof(TestingLogger.Log), new[] { typeof(LogLevel), typeof(EventId), typeof(string), typeof(Exception) }) ?? throw new InvalidOperationException("Cannot find TestingLogger.Log method.");
        private static readonly MethodInfo _regexIsMatchMethod = typeof(Regex).GetMethod(nameof(Regex.IsMatch), new[] { typeof(string), typeof(string) }) ?? throw new InvalidOperationException("Cannot find Regex.IsMatch method.");

        // Marker expressions. If _exceptionExpression is set to one of these, the It.Is method
        // will *not* used.
        private static readonly Expression<Func<object?>> _nullExceptionExpression = () => null; // A null constant exception expression will be used
        private static readonly Expression<Func<object?>> _itIsNotNullExpression = () => null; // The It.IsNotNull() method will be used

        // Specific values. If not null, the value is passed directly to the Log method parameter
        // as a Constant Expression.
        // These have the highest precedence - if they and lower precedence values are both
        // non-null, only the specific value will apply.
        private LogLevel? _logLevel;
        private EventId? _eventId;
        private string? _message;

        // If not null, It.IsRegex(_messageRegexPattern) is passed to the Log method message
        // parameter.
        // NOTE: _messageRegexPattern has a lower precedence than _message and a higher precedence
        // than _messageExpression - if more than one message value is non-null, only the highest
        // precedence value will apply.
        private string? _messageRegexPattern;

        // Lambda expressions passed to the It.Is(expression) method.
        // These have the lowest precedence - if they and any other corresponding values are both
        // non-null, the lambda expression will never apply.
        private Expression<Func<LogLevel, bool>>? _logLevelExpression;
        private Expression<Func<EventId, bool>>? _eventIdExpression;
        private Expression<Func<string, bool>>? _messageExpression;
        
        // The lambda expression 
        private LambdaExpression? _exceptionExpression;

        // The specific exception type to be used with _exceptionExpression. The base Exception
        // type is used if this is null.
        private Type? _exceptionType;

        /// <summary>
        /// Verify that the log was made at <see cref="LogLevel.Trace"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtTrace() => AtLogLevel(LogLevel.Trace);

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Debug"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtDebug() => AtLogLevel(LogLevel.Debug);

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Information"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtInformation() => AtLogLevel(LogLevel.Information);

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Warning"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtWarning() => AtLogLevel(LogLevel.Warning);

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Error"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtError() => AtLogLevel(LogLevel.Error);

        /// <summary>
        /// Verify that a log was made at <see cref="LogLevel.Critical"/>.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtCritical() => AtLogLevel(LogLevel.Critical);

        /// <summary>
        /// Verify that a log was made at the specified <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtLogLevel(LogLevel? logLevel)
        {
            _logLevel = logLevel;
            _logLevelExpression = null;
            return this;
        }

        /// <summary>
        /// Verify that a log was made at the level specified by the
        /// <paramref name="matchLogLevel"/> expression.
        /// </summary>
        /// <param name="matchLogLevel">
        /// An expression that defines whether a <see cref="LogLevel"/> is a match for
        /// verification.
        /// </param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> AtLogLevel(Expression<Func<LogLevel, bool>>? matchLogLevel)
        {
            _logLevel = null;
            _logLevelExpression = matchLogLevel;
            return this;
        }

        /// <summary>
        /// Verify that a log was made with the specified event ID.
        /// </summary>
        /// <param name="eventId">The expected event ID.</param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithEventId(EventId? eventId)
        {
            _eventId = eventId;
            _eventIdExpression = null;
            return this;
        }

        /// <summary>
        /// Verify that a log was made with the event ID specified by the
        /// <paramref name="matchEventId"/> expression.
        /// </summary>
        /// <param name="matchEventId">
        /// An expression that defines whether an <see cref="EventId"/> is a match for
        /// verification.
        /// </param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithEventId(Expression<Func<EventId, bool>>? matchEventId)
        {
            _eventId = null;
            _eventIdExpression = matchEventId;
            return this;
        }

        /// <summary>
        /// Verify that a log was made with the specified message.
        /// </summary>
        /// <param name="message">The expected message.</param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithMessage(string? message)
        {
            _message = message;
            _messageRegexPattern = null;
            _messageExpression = null;
            return this;
        }

        /// <summary>
        /// Verify that a log was made with a message matching the specified regular expression
        /// pattern.
        /// </summary>
        /// <param name="messageRegexPattern">The regular expression pattern.</param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithMessageRegex(string? messageRegexPattern)
        {
            _message = null;
            _messageRegexPattern = messageRegexPattern;
            _messageExpression = null;
            return this;
        }

        /// <summary>
        /// Verify that a log was made with the message specified by the
        /// <paramref name="matchMessage"/> expression.
        /// </summary>
        /// <param name="matchMessage">
        /// An expression that defines whether a message is a match for verification.
        /// </param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithMessage(Expression<Func<string, bool>>? matchMessage)
        {
            _message = null;
            _messageRegexPattern = null;
            _messageExpression = matchMessage;
            return this;
        }

        /// <summary>
        /// Verify that a log was made without an exception.
        /// </summary>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithoutException()
        {
            _exceptionExpression = _nullExceptionExpression;
            _exceptionType = null;
            return this;
        }

        /// <summary>
        /// Verify that a log was made with an exception.
        /// </summary>
        /// <param name="exceptionType">The type of expected exception.</param>
        /// <param name="message">The message of the expected exception.</param>
        /// <param name="messageRegex">
        /// A regular expression pattern that matches the message of the expected exception.
        /// </param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="exceptionType"/> is not assignable to the <see cref="Exception"/> type.
        /// </exception>
        public LogInvocationQuery<TLogger> WithException(
            Type? exceptionType = null,
            string? message = null,
            string? messageRegex = null)
        {
            var exceptionParameter = Expression.Parameter(exceptionType ?? typeof(Exception), "ex");
            Expression body;

            if (exceptionType != null)
            {
                if (!typeof(Exception).IsAssignableFrom(exceptionType))
                    throw new ArgumentOutOfRangeException(nameof(exceptionType), "Must be assignable to the Exception type.");

                _exceptionType = exceptionType;

                if (message != null)
                {
                    body = Expression.Equal(
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(message));
                }
                else if (messageRegex != null)
                {
                    body = Expression.Call(
                        _regexIsMatchMethod,
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(messageRegex));
                }
                else
                {
                    _exceptionExpression = _itIsNotNullExpression;
                    return this;
                }
            }
            else
            {
                _exceptionType = null;

                if (message != null)
                {
                    return WithException(message);
                }
                else if (messageRegex != null)
                {
                    body = Expression.Call(
                        _regexIsMatchMethod,
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(messageRegex));
                }
                else
                {
                    _exceptionExpression = _itIsNotNullExpression;
                    return this;
                }
            }

            body = Expression.AndAlso(
                Expression.NotEqual(exceptionParameter, Expression.Constant(null, typeof(Exception))),
                body);

            _exceptionExpression = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(exceptionType ?? typeof(Exception), typeof(bool)),
                body,
                exceptionParameter);
            return this;
        }

        /// <summary>
        /// Verify that a log was made with an exception.
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithException<TException>()
            where TException : Exception
        {
            _exceptionExpression = _itIsNotNullExpression;
            _exceptionType = typeof(TException);
            return this;
        }

        /// <summary>
        /// Verify that a log was made with an exception with the specified message.
        /// </summary>
        /// <param name="message">The message of the expected exception.</param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public LogInvocationQuery<TLogger> WithException(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            var exceptionParameter = Expression.Parameter(typeof(Exception), "ex");

            _exceptionExpression = Expression.Lambda<Func<Exception, bool>>(
                Expression.AndAlso(
                    Expression.NotEqual(exceptionParameter, Expression.Constant(null, typeof(Exception))),
                    Expression.Equal(
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(message))),
                exceptionParameter);
            _exceptionType = null;

            return this;
        }

        /// <summary>
        /// Verify that a log was made with an exception with the specified message.
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <param name="message">The message of the expected exception.</param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public LogInvocationQuery<TLogger> WithException<TException>(string message)
            where TException : Exception
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            var exceptionParameter = Expression.Parameter(typeof(TException), "ex");

            _exceptionExpression = Expression.Lambda<Func<TException, bool>>(
                Expression.AndAlso(
                    Expression.NotEqual(exceptionParameter, Expression.Constant(null, typeof(Exception))),
                    Expression.Equal(
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(message))),
                exceptionParameter);
            _exceptionType = typeof(TException);

            return this;
        }

        /// <summary>
        /// Verify that a log was made with an exception specified by the
        /// <paramref name="matchException"/> expression.
        /// </summary>
        /// <param name="matchException">
        /// An expression that defines whether an exception is a match for verification.
        /// </param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithException(Expression<Func<Exception, bool>>? matchException)
        {
            return WithException<Exception>(matchException);
        }

        /// <summary>
        /// Verify that a log was made with an exception specified by the
        /// <paramref name="matchException"/> expression.
        /// </summary>
        /// <typeparam name="TException">The type of the expected exception.</typeparam>
        /// <param name="matchException">
        /// An expression that defines whether an exception is a match for verification.
        /// </param>
        /// <returns>The same instance of <see cref="LogInvocationQuery{TLogger}"/>.</returns>
        public LogInvocationQuery<TLogger> WithException<TException>(Expression<Func<TException, bool>>? matchException)
            where TException : Exception
        {
            _exceptionExpression = matchException;
            _exceptionType = typeof(TException);
            return this;
        }

        internal void Verify(Mock<TLogger> mockLogger, Times? times = null, string? failMessage = null)
        {
            times ??= Times.AtLeastOnce();

            var loggerParameter = Expression.Parameter(typeof(TLogger), "logger");
            var logLevelArgument = GetLogLevelArgument();
            var eventIdArgument = GetEventIdArgument();
            var messageArgument = GetMessageArgument();
            var exceptionArgument = GetExceptionArgument();

            var logExpression = Expression.Lambda<Action<TLogger>>(
                Expression.Call(loggerParameter, _testingLoggerlogMethod, logLevelArgument, eventIdArgument, messageArgument, exceptionArgument),
                loggerParameter);

            mockLogger.Verify(logExpression, times.Value, failMessage);
        }

        private static bool HasExpressionOfFuncOfTValueToBoolParameter(MethodInfo method)
        {
            if (method.Name != nameof(It.Is))
                return false;

            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                return false;

            var parameter = parameters[0];
            if (!parameter.ParameterType.IsGenericType
                || parameter.ParameterType.GetGenericTypeDefinition() != typeof(Expression<>))
                return false;

            var delegateType = parameter.ParameterType.GetGenericArguments()[0];
            if (!delegateType.IsGenericType
                || delegateType.GetGenericTypeDefinition() != typeof(Func<,>))
                return false;

            var delegateGenericArguments = delegateType.GetGenericArguments();
            var delegateParameterType = delegateGenericArguments[0];
            var delegateReturnType = delegateGenericArguments[1];

            return delegateParameterType.IsGenericParameter && delegateReturnType == typeof(bool);
        }

        private Expression GetLogLevelArgument()
        {
            if (_logLevel != null)
                return Expression.Constant(_logLevel.Value);
            else if (_logLevelExpression != null)
                return Expression.Call(_itIsMethod.MakeGenericMethod(typeof(LogLevel)), Expression.Constant(_logLevelExpression));
            else
                return Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(LogLevel)));
        }

        private Expression GetEventIdArgument()
        {
            if (_eventId != null)
                return Expression.Constant(_eventId.Value);
            else if (_eventIdExpression != null)
                return Expression.Call(_itIsMethod.MakeGenericMethod(typeof(EventId)), Expression.Constant(_eventIdExpression));
            else
                return Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(EventId)));
        }

        private Expression GetMessageArgument()
        {
            if (_message != null)
                return Expression.Constant(_message);
            else if (_messageRegexPattern != null)
                return Expression.Call(_itIsRegexMethod, Expression.Constant(_messageRegexPattern));
            else if (_messageExpression != null)
                return Expression.Call(_itIsMethod.MakeGenericMethod(typeof(string)), Expression.Constant(_messageExpression));
            else
                return Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(string)));
        }

        private Expression GetExceptionArgument()
        {
            if (_exceptionExpression != null)
            {
                if (_exceptionExpression == _nullExceptionExpression)
                    return Expression.Constant(null, typeof(Exception));
                else if (_exceptionExpression == _itIsNotNullExpression)
                    return Expression.Call(_itIsNotNullMethod.MakeGenericMethod(_exceptionType ?? typeof(Exception)));
                else
                    return Expression.Call(_itIsMethod.MakeGenericMethod(_exceptionType ?? typeof(Exception)), Expression.Constant(_exceptionExpression));
            }
            else
            {
                return Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(Exception)));
            }
        }
    }
}
