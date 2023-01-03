using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Moq
{
    internal class LogInvocationQuery<TLogger> : ILogInvocationQuery<TLogger>
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

        // Either the lambda expression passed to the It.Is(expression) method, or one of the marker
        // expressions: _nullExceptionExpression or _itIsNotNullExpression.
        private LambdaExpression? _exceptionExpression;

        // The specific exception type to be used with _exceptionExpression. The base Exception
        // type is used if this is null.
        private Type? _exceptionType;

        public ILogInvocationQuery<TLogger> AtTrace() => AtLogLevel(LogLevel.Trace);

        public ILogInvocationQuery<TLogger> AtDebug() => AtLogLevel(LogLevel.Debug);

        public ILogInvocationQuery<TLogger> AtInformation() => AtLogLevel(LogLevel.Information);

        public ILogInvocationQuery<TLogger> AtWarning() => AtLogLevel(LogLevel.Warning);

        public ILogInvocationQuery<TLogger> AtError() => AtLogLevel(LogLevel.Error);

        public ILogInvocationQuery<TLogger> AtCritical() => AtLogLevel(LogLevel.Critical);

        public ILogInvocationQuery<TLogger> AtLogLevel(LogLevel? logLevel)
        {
            _logLevel = logLevel;
            _logLevelExpression = null;
            return this;
        }

        public ILogInvocationQuery<TLogger> AtLogLevel(Expression<Func<LogLevel, bool>>? matchLogLevel)
        {
            _logLevel = null;
            _logLevelExpression = matchLogLevel;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithEventId(EventId? eventId)
        {
            _eventId = eventId;
            _eventIdExpression = null;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithEventId(Expression<Func<EventId, bool>>? matchEventId)
        {
            _eventId = null;
            _eventIdExpression = matchEventId;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithMessage(string? message)
        {
            _message = message;
            _messageRegexPattern = null;
            _messageExpression = null;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithMessageRegex(string? messageRegexPattern)
        {
            _message = null;
            _messageRegexPattern = messageRegexPattern;
            _messageExpression = null;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithMessage(Expression<Func<string, bool>>? matchMessage)
        {
            _message = null;
            _messageRegexPattern = null;
            _messageExpression = matchMessage;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithoutException()
        {
            _exceptionExpression = _nullExceptionExpression;
            _exceptionType = null;
            return this;
        }

        public ILogInvocationQuery<TLogger> WithException(
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

        public ILogInvocationQuery<TLogger> WithException<TException>()
            where TException : Exception
        {
            _exceptionExpression = _itIsNotNullExpression;
            _exceptionType = typeof(TException);
            return this;
        }

        public ILogInvocationQuery<TLogger> WithException(string message)
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

        public ILogInvocationQuery<TLogger> WithException<TException>(string message)
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

        public ILogInvocationQuery<TLogger> WithException(Expression<Func<Exception, bool>>? matchException)
        {
            return WithException<Exception>(matchException);
        }

        public ILogInvocationQuery<TLogger> WithException<TException>(Expression<Func<TException, bool>>? matchException)
            where TException : Exception
        {
            _exceptionExpression = matchException;
            _exceptionType = typeof(TException);
            return this;
        }

        /// <summary>
        /// Verifies that the specified mock logger performed the logging invocation defined by this instance of
        /// <see cref="LogInvocationQuery{TLogger}"/>.
        /// </summary>
        /// <param name="mockLogger">The mock logger to verify.</param>
        /// <param name="times">The number of times the logging is expected to be called. If <see langword="null"/>,
        ///     <see cref="Times.AtLeastOnce"/> is used instead.</param>
        /// <param name="failMessage">Message to show if verification fails.</param>
        public void Verify(Mock<TLogger> mockLogger, Times? times = null, string? failMessage = null)
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
