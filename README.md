# RandomSkunk.Logging.Testing.Moq

*Mock logger implementations for Moq and Microsoft.Extensions.Logging.*

The `Microsoft.Extensions.Logging.ILogger` interface is difficult to mock - its `Log` method is generic, but the extension methods that call it (e.g. `LogError`) pass an internal type as the generic argument. This means there's no good way to know if most logging has taken place. This library makes it easy to do this kind of verification.

## Usage

```c#
// Create a mock logger.
var mockLogger = new MockLogger();

// Do some logging.
mockLogger.Object.LogInformation("Hello, {who}!", "world");

// Verify that the logging occurred.
mockLogger.Verify(m => m.Log(
    LogLevel.Information,
    It.IsAny<EventId>(),
    "Hello, world!",
    null));
```

Verifying a method with four parameters (i.e. the `Log` method) can be a bit tedious. To lessen the tedium, the `VerifyLog` extension method is provided.

```c#
mockLogger.VerifyLog(
    log => log.AtInformation().WithMessage("Hello, world!").WithoutException());
```

Similarly, setting up the `Log` method (e.g. to set a callback method) can be tedious. The `SetupLog` extension method is provided to help:

```c#
mockLogger.SetupLog()
    .Callback<LogLevel, EventId, string, Exception>(
        (logLevel, eventId, message, exception) =>
        {
            // TODO: Handle the callback.
        });
```
