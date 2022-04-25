using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Moq
{
    /// <summary>
    /// Defines a mock logger.
    /// </summary>
    /// <typeparam name="TCategoryName">
    /// The type whose name is used for the logger category name.
    /// </typeparam>
    public class MockLogger<TCategoryName> : Mock<TestingLogger<TCategoryName>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockLogger{TCategoryName}"/> class.
        /// </summary>
        public MockLogger()
        {
            CallBase = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockLogger{TCategoryName}"/> class.
        /// </summary>
        /// <param name="logLevel">The log level of the logger.</param>
        public MockLogger(LogLevel logLevel)
            : base(logLevel)
        {
            CallBase = true;
        }

        /// <summary>
        /// Gets the list of mock objects that correspond to calls to
        /// <see cref="TestingLogger.BeginScope"/>. The first item in the list corresponds to the
        /// first call, the second item to the second call, and so on.
        /// </summary>
        public IReadOnlyList<MockScopeCompletion> ScopeCompletions => Object.ScopeCompletions;
    }
}
