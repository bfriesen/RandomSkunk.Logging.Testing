using System;

namespace Moq
{
    /// <summary>
    /// A mock disposable object returned from calls to <see cref="TestingLogger.BeginScope"/>.
    /// </summary>
    public abstract class MockScopeCompletion : Mock<IDisposable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockScopeCompletion"/> class.
        /// </summary>
        /// <param name="state">The identifier for the logical operation scope.</param>
        protected MockScopeCompletion(object? state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the identifier for the logical operation scope.
        /// </summary>
        public object? State { get; }
    }
}
