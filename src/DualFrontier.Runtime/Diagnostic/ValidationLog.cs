namespace DualFrontier.Runtime.Diagnostic;

/// <summary>
/// Thread-safe ring buffer для Vulkan validation messages. Receives writes from
/// `ValidationLayer.DebugCallback` (potentially от any Vulkan driver thread); reads serve
/// diagnostic dumps + V0.A smoke test exit criteria check (ErrorCount == 0).
/// </summary>
public sealed class ValidationLog
{
    private const int MaxStoredMessages = 1024;
    private readonly object _lock = new();
    private readonly Queue<ValidationMessage> _messages = new();

    public int ErrorCount { get; private set; }
    public int WarningCount { get; private set; }
    public int InfoCount { get; private set; }

    public void Record(ValidationSeverity severity, string message)
    {
        lock (_lock)
        {
            if (_messages.Count >= MaxStoredMessages)
            {
                _messages.Dequeue();
            }
            _messages.Enqueue(new ValidationMessage(severity, message, DateTime.UtcNow));

            switch (severity)
            {
                case ValidationSeverity.Error:
                    ErrorCount++;
                    break;
                case ValidationSeverity.Warning:
                    WarningCount++;
                    break;
                case ValidationSeverity.Info:
                    InfoCount++;
                    break;
            }
        }
    }

    public IReadOnlyList<ValidationMessage> Snapshot()
    {
        lock (_lock)
        {
            return _messages.ToArray();
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _messages.Clear();
            ErrorCount = 0;
            WarningCount = 0;
            InfoCount = 0;
        }
    }
}

public readonly record struct ValidationMessage(
    ValidationSeverity Severity,
    string Message,
    DateTime TimestampUtc);

public enum ValidationSeverity
{
    Info,
    Warning,
    Error,
}
