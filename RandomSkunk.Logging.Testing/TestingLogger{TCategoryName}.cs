namespace Microsoft.Extensions.Logging
{
    public abstract class TestingLogger<TCategoryName> : TestingLogger, ILogger<TCategoryName>
    {
        protected TestingLogger()
            : base()
        {
        }

        protected TestingLogger(LogLevel logLevel)
            : base(logLevel)
        {
        }
    }
}
