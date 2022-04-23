using Moq;
using System;

namespace Microsoft.Extensions.Logging
{
    public class MockTestingLogger<TCategoryName> : Mock<TestingLogger<TCategoryName>>
    {
        public MockTestingLogger()
        {
            CallBase = true;
        }

        public MockTestingLogger(LogLevel logLevel)
            : base(logLevel)
        {
            CallBase = true;
        }
    }
}
