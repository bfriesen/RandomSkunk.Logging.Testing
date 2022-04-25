using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Moq
{
    /// <summary>
    /// Defines a mock logger.
    /// </summary>
    public class MockLogger : Mock<TestingLogger>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockLogger"/> class.
        /// </summary>
        public MockLogger()
        {
            CallBase = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockLogger"/> class.
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
