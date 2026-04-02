using SentinelStream.Models;

namespace SentinelStream.Core.Agora;

/// <summary>
/// Event arguments for when a log message is received via Agora Data Stream or WebSocket.
/// </summary>
public class LogReceivedEventArgs : EventArgs
{
    public LogEntry Log { get; }
    public LogReceivedEventArgs(LogEntry log) => Log = log;
}

/// <summary>
/// Event arguments for session state changes.
/// </summary>
public class SessionStateChangedEventArgs : EventArgs
{
    public SessionState OldState { get; }
    public SessionState NewState { get; }
    public string? Message { get; }

    public SessionStateChangedEventArgs(SessionState oldState, SessionState newState, string? message = null)
    {
        OldState = oldState;
        NewState = newState;
        Message = message;
    }
}

/// <summary>
/// Event arguments for participant changes.
/// </summary>
public class ParticipantChangedEventArgs : EventArgs
{
    public string UserId { get; }
    public string DisplayName { get; }
    public ParticipantAction Action { get; }

    public ParticipantChangedEventArgs(string userId, string displayName, ParticipantAction action)
    {
        UserId = userId;
        DisplayName = displayName;
        Action = action;
    }
}

public enum SessionState
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
    Error
}

public enum ParticipantAction
{
    Joined,
    Left
}

/// <summary>
/// Agora RTC Service Wrapper — manages the lifecycle of a War Room session.
/// 
/// NOTE: This is a high-level abstraction layer. The actual Agora SDK bindings
/// (AgoraRtcEngine) must be installed via NuGet: Agora.RTC.SDK
/// For now, this provides the full interface contract and simulation logic
/// so the UI and services can be developed in parallel.
/// </summary>
public class AgoraWarRoomClient : IDisposable
{
    private SessionState _currentState = SessionState.Disconnected;
    private string _channelName = string.Empty;
    private readonly string _appId;
    private readonly List<SessionParticipant> _participants = new();
    private bool _disposed;

    // Events for UI binding
    public event EventHandler<SessionStateChangedEventArgs>? StateChanged;
    public event EventHandler<ParticipantChangedEventArgs>? ParticipantChanged;
    public event EventHandler<LogReceivedEventArgs>? LogReceived;
    public event EventHandler<string>? ChatMessageReceived;

    public SessionState CurrentState => _currentState;
    public string ChannelName => _channelName;
    public IReadOnlyList<SessionParticipant> Participants => _participants.AsReadOnly();

    public AgoraWarRoomClient(string appId)
    {
        if (string.IsNullOrWhiteSpace(appId))
            throw new ArgumentException("Agora App ID is required.", nameof(appId));

        _appId = appId;
    }

    /// <summary>
    /// Joins a War Room channel. In production, this initializes the Agora RTC engine,
    /// sets up E2EE, and joins the specified channel.
    /// </summary>
    public async Task JoinWarRoomAsync(string channelName, string userId, string displayName, string? token = null)
    {
        if (_currentState == SessionState.Connected)
            throw new InvalidOperationException("Already connected to a War Room.");

        UpdateState(SessionState.Connecting, $"Connecting to War Room: {channelName}...");

        _channelName = channelName;

        // --- AGORA SDK INTEGRATION POINT ---
        // In production, this is where you would:
        // 1. Initialize AgoraRtcEngine with _appId
        // 2. Set channel profile to Communication
        // 3. Enable encryption with AES-256-GCM
        // 4. Join channel with token
        //
        // Example (when Agora SDK is installed):
        // _rtcEngine = AgoraRtcEngine.CreateAgoraRtcEngine();
        // _rtcEngine.Initialize(new RtcEngineContext(_appId));
        // _rtcEngine.EnableEncryption(true, new EncryptionConfig { 
        //     encryptionMode = ENCRYPTION_MODE.AES_256_GCM2, 
        //     encryptionKey = encryptionKey 
        // });
        // _rtcEngine.JoinChannel(token, channelName, "", uid);
        // ---

        await Task.Delay(500); // Simulate connection handshake

        var participant = new SessionParticipant
        {
            UserId = userId,
            DisplayName = displayName,
            Role = ParticipantRole.Analyst,
            JoinedAt = DateTime.UtcNow
        };
        _participants.Add(participant);

        UpdateState(SessionState.Connected, $"Connected to War Room: {channelName}");
        ParticipantChanged?.Invoke(this, new ParticipantChangedEventArgs(userId, displayName, ParticipantAction.Joined));
    }

    /// <summary>
    /// Leaves the current War Room and cleans up resources.
    /// </summary>
    public async Task LeaveWarRoomAsync()
    {
        if (_currentState != SessionState.Connected)
            return;

        // --- AGORA SDK INTEGRATION POINT ---
        // _rtcEngine?.LeaveChannel();
        // _rtcEngine?.Dispose();
        // ---

        await Task.Delay(200); // Simulate disconnect

        _participants.Clear();
        UpdateState(SessionState.Disconnected, "Left War Room.");
        _channelName = string.Empty;
    }

    /// <summary>
    /// Sends a chat message to the War Room via Agora RTM or Data Stream.
    /// </summary>
    public async Task SendChatMessageAsync(string message)
    {
        if (_currentState != SessionState.Connected)
            throw new InvalidOperationException("Not connected to a War Room.");

        // --- AGORA SDK INTEGRATION POINT ---
        // _rtcEngine?.SendStreamMessage(streamId, Encoding.UTF8.GetBytes(message));
        // ---

        await Task.Delay(50);
        ChatMessageReceived?.Invoke(this, message);
    }

    /// <summary>
    /// Simulates receiving a log entry (in production, this comes via Agora Data Stream).
    /// </summary>
    public void InjectLogEntry(LogEntry log)
    {
        LogReceived?.Invoke(this, new LogReceivedEventArgs(log));
    }

    private void UpdateState(SessionState newState, string? message = null)
    {
        var oldState = _currentState;
        _currentState = newState;
        StateChanged?.Invoke(this, new SessionStateChangedEventArgs(oldState, newState, message));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            // --- AGORA SDK INTEGRATION POINT ---
            // _rtcEngine?.Dispose();
            // ---
            _participants.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
