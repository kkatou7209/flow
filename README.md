# Flow : Logger in .NET

Flow is a lightweight .NET logging library focused on simple file logging.

Flow provides minimum functionality to do logging. 

## Features

- File base logging
- Format logging based on object types
- Fun-out logging

## Usage

### Basic logger creation

To create logger, use `FlowKit` static class.

All logger implements `IDisposable`, so you should loggers with `using` or manually call `Dispose()`.

```csharp
using Flow;

using Logger logger = FlowKit.Create("~/Logs/ap.log");
```

You can create a logger from any `Stream` and `TextWriter`.

```csharp
Stream stream = new MemoryStream();

using Logger logger = FlowKit.Create(stream);
```

```csharp
TextWriter console = Console.Out;

using Logger logger = FlowKit.Create(console);
```

If want to format object, use the generic one.

```csharp
using Flow;

public class LogInfo
{
    ...
}

using FormatLogger<LogInfo> logger = FlowKit.Create<LogInfo>(
    info => $"[{info.LoggedOn.ToString(HH:mm:ss)}] {info.State}",
    "~/Logs/ap.log"
);
```

You can create fan-out logger with combining loggers using `Combine` method.

```csharp
Logger logger = ...;
FormatLogger formatLogger = ...;

using FanOutLogger fanOutLogger FloKit.Combine(logger, formatLogger);
```

### Buffer size and capacity

The loggers do not write logs immediately, but store those to inner buffer instead.

You can set it's capacity passing to parameters.

```csharp
using Logger logger = FlowKit.Create("~/Logs/app.log", capacity: 40_000);
```

```csharp
using FormatLogger<LogInfo> logger = FlowKit.Create<LogInfo>(
    info => info.ToString(),
    "~/Logs/app.log", capacity: 40_000
);
```

## License

[MIT](LICENSE)