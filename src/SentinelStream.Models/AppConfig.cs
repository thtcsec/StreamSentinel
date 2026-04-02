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
    public string LogServerUrl { get; set; } = "ws://localhost:8000/ws/logs";
}
