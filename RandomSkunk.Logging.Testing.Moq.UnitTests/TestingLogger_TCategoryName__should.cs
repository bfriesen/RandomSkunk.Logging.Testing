using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Moq.UnitTests
{
    public class TestingLogger_TCategoryName__should
    {
        [Fact]
        public void Set_LogLevel_to_trace_in_default_constructor()
        {
            TestingLogger<TestingLogger_TCategoryName__should> logger = new ConcreteTestingLogger<TestingLogger_TCategoryName__should>();

            logger.LogLevel.Should().Be(LogLevel.Trace);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        public void Set_LogLevel_in_constructor(LogLevel logLevel)
        {
            TestingLogger<TestingLogger_TCategoryName__should> logger = new ConcreteTestingLogger<TestingLogger_TCategoryName__should>(logLevel);

            logger.LogLevel.Should().Be(logLevel);
        }

        public class ConcreteTestingLogger<TCategoryName> : TestingLogger<TCategoryName>
        {
            public ConcreteTestingLogger()
            {
            }

            public ConcreteTestingLogger(LogLevel logLevel)
                : base(logLevel)
            {
            }

            public override void Log(LogLevel logLevel, EventId eventId, string message, Exception? exception)
            {
            }
        }
    }
}
