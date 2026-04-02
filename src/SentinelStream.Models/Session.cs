namespace SentinelStream.Models;

/// <summary>
/// Represents a War Room session for incident response.
/// </summary>
public class WarRoomSession
{
    public string SessionId { get; set; } = string.Empty;
    public string ChannelName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public List<SessionParticipant> Participants { get; set; } = new();
}

public class SessionParticipant
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; } = ParticipantRole.Analyst;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public enum SessionStatus
{
    Active,
    Paused,
    Terminated
}

public enum ParticipantRole
{
    Lead,
    Analyst,
    Observer
}
