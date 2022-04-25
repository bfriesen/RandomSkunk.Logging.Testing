using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Moq.UnitTests
{
    public class MockLogger_TCategoryName__should
    {
        [Fact]
        public void Set_CallBase_to_true_and_LogLevel_to_trace_in_default_constructor()
        {
            var mockLogger = new MockLogger<MockLogger_TCategoryName__should>();

            mockLogger.CallBase.Should().BeTrue();
            mockLogger.Object.LogLevel.Should().Be(LogLevel.Trace);
        }

        [Theory]
        [InlineData(LogLevel.Trace)]
        public void Set_CallBase_to_true_and_LogLevel_in_constructor(LogLevel logLevel)
        {
            var mockLogger = new MockLogger<MockLogger_TCategoryName__should>(logLevel);

            mockLogger.CallBase.Should().BeTrue();
            mockLogger.Object.LogLevel.Should().Be(logLevel);
        }

        [Fact]
        public void Expose_the_ScopeCompletions_property_from_its_mock_object()
        {
            var mockLogger = new MockLogger<MockLogger_TCategoryName__should>();

            mockLogger.ScopeCompletions.Should().BeSameAs(mockLogger.Object.ScopeCompletions);
        }
    }
}
