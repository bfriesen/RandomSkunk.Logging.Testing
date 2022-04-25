namespace Moq
{
    /// <summary>
    /// A mock disposable object returned from calls to <see cref="TestingLogger.BeginScope"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    public class MockScopeCompletion<TState> : MockScopeCompletion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockScopeCompletion{TState}"/> class.
        /// </summary>
        /// <param name="state">The identifier for the logical operation scope.</param>
        public MockScopeCompletion(TState state)
            : base(state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the identifier for the logical operation scope.
        /// </summary>
        public new TState State { get; }
    }
}
