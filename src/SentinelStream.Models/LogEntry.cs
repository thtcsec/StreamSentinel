namespace SentinelStream.Models;

/// <summary>
/// Represents a single log entry received from the monitoring agent.
/// </summary>
public class LogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public LogSeverity Severity { get; set; } = LogSeverity.Info;
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RawData { get; set; }
}

public enum LogSeverity
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}
