namespace SentinelStream.Models;

/// <summary>
/// Application configuration loaded from .env file.
/// </summary>
public class AppConfig
{
    public string AgoraAppId { get; set; } = string.Empty;
    public string AgoraAppCertificate { get; set; } = string.Empty;
    public string EncryptionKey { get; set; } = string.Empty;
    public string ForensicSalt { get; set; } = string.Empty;

    /// <summary>When true (and FORENSIC_SALT is set), session log text is written and hashed on leave.</summary>
    public bool ExportSessionLogOnLeave { get; set; }

    /// <summary>Optional folder for session exports; empty = temp directory.</summary>
    public string SessionExportDirectory { get; set; } = string.Empty;

    /// <summary>WebSocket URL for the monitoring agent (ws:// or wss://). Empty = no agent connection.</summary>
    public string LogServerUrl { get; set; } = string.Empty;

    /// <summary>When true, the war room shows a simulated log feed (for demos without an agent).</summary>
    public bool EnableDemoLogFeed { get; set; } = true;

    /// <summary>Maps this config to feed options the war room consumes.</summary>
    public WarRoomFeedOptions ToFeedOptions()
    {
        Uri? logUri = null;
        if (!string.IsNullOrWhiteSpace(LogServerUrl) &&
            Uri.TryCreate(LogServerUrl.Trim(), UriKind.Absolute, out var u) &&
            (u.Scheme == "ws" || u.Scheme == "wss"))
        {
            logUri = u;
        }

        return new WarRoomFeedOptions
        {
            LogAgentWebSocketUri = logUri,
            EnableDemoLogFeed = EnableDemoLogFeed
        };
    }

    public SessionArtifactOptions ToSessionArtifactOptions() => new()
    {
        ExportSessionLogOnLeave = ExportSessionLogOnLeave,
        ForensicSalt = string.IsNullOrWhiteSpace(ForensicSalt) ? null : ForensicSalt.Trim(),
        ExportDirectory = string.IsNullOrWhiteSpace(SessionExportDirectory)
            ? null
            : SessionExportDirectory.Trim()
    };
}
