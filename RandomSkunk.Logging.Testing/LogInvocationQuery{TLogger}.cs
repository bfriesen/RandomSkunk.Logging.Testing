using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Logging
{
    public class LogInvocationQuery<TLogger>
        where TLogger : TestingLogger
    {
        private static readonly MethodInfo _itIsAnyMethod = typeof(It).GetMethod(nameof(It.IsAny));
        private static readonly MethodInfo _itIsRegexMethod = typeof(It).GetMethod(nameof(It.IsRegex), new[] { typeof(string) });
        private static readonly MethodInfo _itIsMethod = typeof(It).GetMethods().Single(HasExpressionOfFuncOfTValueToBoolParameter);
        private static readonly MethodInfo _logMethod = typeof(TestingLogger).GetMethod(nameof(TestingLogger.Log), new[] { typeof(LogLevel), typeof(EventId), typeof(string), typeof(Exception) });
        private static readonly MethodInfo _regexIsMatchMethod = typeof(Regex).GetMethod(nameof(Regex.IsMatch), new[] { typeof(string), typeof(string) });

        private LogLevel? _logLevel;
        private EventId? _eventId;
        private string? _message;
        private string? _messageRegexPattern;
        private LambdaExpression? _exceptionMatchesExpression;

        public LogInvocationQuery<TLogger> AtTrace() => AtLogLevel(LogLevel.Trace);

        public LogInvocationQuery<TLogger> AtDebug() => AtLogLevel(LogLevel.Debug);

        public LogInvocationQuery<TLogger> AtInformation() => AtLogLevel(LogLevel.Information);

        public LogInvocationQuery<TLogger> AtWarning() => AtLogLevel(LogLevel.Warning);

        public LogInvocationQuery<TLogger> AtError() => AtLogLevel(LogLevel.Error);

        public LogInvocationQuery<TLogger> AtCritical() => AtLogLevel(LogLevel.Critical);

        public LogInvocationQuery<TLogger> AtLogLevel(LogLevel? logLevel)
        {
            _logLevel = logLevel;
            return this;
        }

        public LogInvocationQuery<TLogger> WithEventId(EventId? eventId)
        {
            _eventId = eventId;
            return this;
        }

        public LogInvocationQuery<TLogger> WithMessage(string? message)
        {
            _message = message;
            return this;
        }

        public LogInvocationQuery<TLogger> WithMessageRegex(string? messageRegexPattern)
        {
            _messageRegexPattern = messageRegexPattern;
            return this;
        }

        internal LogInvocationQuery<TLogger> WithException(Type? exceptionType, string? exceptionMessage, string? exceptionMessageRegex)
        {
            var exceptionParameter = Expression.Parameter(exceptionType ?? typeof(Exception), "ex");
            Expression body;

            if (exceptionType != null)
            {
                if (!typeof(Exception).IsAssignableFrom(exceptionType))
                    throw new ArgumentOutOfRangeException(nameof(exceptionType), "Must be assignable to the Exception type.");

                if (exceptionMessage != null)
                {
                    body = Expression.Equal(
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(exceptionMessage));
                }
                else if (exceptionMessageRegex != null)
                {
                    body = Expression.Call(
                        _regexIsMatchMethod,
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(exceptionMessageRegex));
                }
                else
                {
                    body = Expression.Constant(true);
                }
            }
            else
            {
                if (exceptionMessage != null)
                {
                    body = Expression.Equal(
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(exceptionMessage));
                }
                else if (exceptionMessageRegex != null)
                {
                    body = Expression.Call(
                        _regexIsMatchMethod,
                        Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                        Expression.Constant(exceptionMessageRegex));
                }
                else
                {
                    _exceptionMatchesExpression = null;
                    return this;
                }
            }

            _exceptionMatchesExpression = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(exceptionType ?? typeof(Exception), typeof(bool)),
                body,
                exceptionParameter);
            return this;
        }

        public LogInvocationQuery<TLogger> WithException()
        {
            Expression<Func<Exception, bool>> exceptionMatchesExpression = ex => true;
            _exceptionMatchesExpression = exceptionMatchesExpression;
            return this;
        }

        public LogInvocationQuery<TLogger> WithException<TException>()
            where TException : Exception
        {
            Expression<Func<TException, bool>> exceptionMatchesExpression = ex => true;
            _exceptionMatchesExpression = exceptionMatchesExpression;
            return this;
        }

        public LogInvocationQuery<TLogger> WithException(string message)
        {
            var exceptionParameter = Expression.Parameter(typeof(Exception), "ex");

            _exceptionMatchesExpression = Expression.Lambda<Func<Exception, bool>>(
                Expression.Equal(
                    Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                    Expression.Constant(message)),
                exceptionParameter);

            return this;
        }

        public LogInvocationQuery<TLogger> WithException<TException>(string message)
            where TException : Exception
        {
            var exceptionParameter = Expression.Parameter(typeof(TException), "ex");

            _exceptionMatchesExpression = Expression.Lambda<Func<TException, bool>>(
                Expression.Equal(
                    Expression.PropertyOrField(exceptionParameter, nameof(Exception.Message)),
                    Expression.Constant(message)),
                exceptionParameter);

            return this;
        }

        public LogInvocationQuery<TLogger> WithException(Expression<Func<Exception, bool>> exceptionMatchesExpression)
        {
            return WithException<Exception>(exceptionMatchesExpression);
        }

        public LogInvocationQuery<TLogger> WithException<TException>(Expression<Func<TException, bool>> exceptionMatchesExpression)
            where TException : Exception
        {
            _exceptionMatchesExpression = exceptionMatchesExpression;
            return this;
        }

        internal void Verify(Mock<TLogger> mockLogger, Times? times, string? failMessage)
        {
            times ??= Times.Once();

            Expression logLevelArgument;
            Expression eventIdArgument;
            Expression messageArgument;
            Expression exceptionArgument;

            if (_logLevel == null)
                logLevelArgument = Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(LogLevel)));
            else
                logLevelArgument = Expression.Constant(_logLevel.Value);

            if (_eventId == null)
                eventIdArgument = Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(EventId)));
            else
                eventIdArgument = Expression.Constant(_eventId.Value);

            if (_message != null)
                messageArgument = Expression.Constant(_message);
            else if (_messageRegexPattern != null)
                messageArgument = Expression.Call(_itIsRegexMethod, Expression.Constant(_messageRegexPattern));
            else
                messageArgument = Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(string)));

            if (_exceptionMatchesExpression != null)
            {
                var exceptionType = _exceptionMatchesExpression.Parameters[0].Type;

                if (_exceptionMatchesExpression.Body.NodeType == ExpressionType.Constant
                    && Equals(((ConstantExpression)_exceptionMatchesExpression.Body).Value, true))
                {
                    exceptionArgument = Expression.Call(_itIsAnyMethod.MakeGenericMethod(exceptionType));
                }
                else
                {
                    exceptionArgument = Expression.Call(
                        _itIsMethod.MakeGenericMethod(exceptionType),
                        Expression.Constant(_exceptionMatchesExpression));
                }
            }
            else
            {
                exceptionArgument = Expression.Call(_itIsAnyMethod.MakeGenericMethod(typeof(Exception)));
            }

            var loggerParameter = Expression.Parameter(typeof(TLogger), "logger");

            var logExpression = Expression.Lambda<Action<TLogger>>(
                Expression.Call(loggerParameter, _logMethod, logLevelArgument, eventIdArgument, messageArgument, exceptionArgument),
                loggerParameter);

            mockLogger.Verify(
                logExpression,
                times.Value,
                failMessage);
        }

        private static bool HasExpressionOfFuncOfTValueToBoolParameter(MethodInfo method)
        {
            if (method.Name == nameof(It.Is))
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var parameter = parameters[0];
                    if (parameter.ParameterType.IsGenericType
                        && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                    {
                        var delegateType = parameter.ParameterType.GetGenericArguments()[0];
                        if (delegateType.IsGenericType
                            && delegateType.GetGenericTypeDefinition() == typeof(Func<,>))
                        {
                            var delegateGenericArguments = delegateType.GetGenericArguments();
                            var delegateParameterType = delegateGenericArguments[0];
                            var delegateReturnType = delegateGenericArguments[1];

                            if (delegateParameterType.IsGenericParameter && delegateReturnType == typeof(bool))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
