using Microsoft.Extensions.Logging;

namespace Moq
{
    /// <summary>
    /// Defines an abstract implementation of the <see cref="ILogger{TCategoryName}"/> interface
    /// meant for testing.
    /// </summary>
    /// <typeparam name="TCategoryName">
    /// The type whose name is used for the logger category name.
    /// </typeparam>
    public abstract class TestingLogger<TCategoryName> : TestingLogger, ILogger<TCategoryName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestingLogger{TCategoryName}"/> class.
        /// </summary>
        protected TestingLogger()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingLogger{TCategoryName}"/> class.
        /// </summary>
        /// <param name="logLevel">The log level of the logger.</param>
        protected TestingLogger(LogLevel logLevel)
            : base(logLevel)
        {
        }
    }
}
