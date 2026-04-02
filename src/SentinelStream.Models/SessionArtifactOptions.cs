namespace SentinelStream.Models;

/// <summary>
/// Optional export of war-room log text + forensic hash when the session ends.
/// </summary>
public sealed class SessionArtifactOptions
{
    /// <summary>When true and <see cref="ForensicSalt"/> is set, a log file is written on leave.</summary>
    public bool ExportSessionLogOnLeave { get; init; }

    /// <summary>Salt passed to <c>ForensicHasher</c>; must be non-empty to export.</summary>
    public string? ForensicSalt { get; init; }

    /// <summary>Directory for exports; null or empty uses the system temp folder.</summary>
    public string? ExportDirectory { get; init; }

    public bool ShouldExport => ExportSessionLogOnLeave && !string.IsNullOrWhiteSpace(ForensicSalt);
}
