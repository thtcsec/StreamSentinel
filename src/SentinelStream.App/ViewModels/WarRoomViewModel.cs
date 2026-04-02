using System.Collections.ObjectModel;
using System.Windows.Input;
using SentinelStream.Core.Agora;
using SentinelStream.Models;

namespace SentinelStream.App.ViewModels;

/// <summary>
/// ViewModel for the War Room dashboard — displays logs, participants, chat, and connection status.
/// </summary>
public class WarRoomViewModel : ViewModelBase, IDisposable
{
    private readonly AgoraWarRoomClient _agoraClient;
    private string _connectionStatus = "Disconnected";
    private string _channelName = string.Empty;
    private string _chatInput = string.Empty;
    private string _displayName = string.Empty;

    public ObservableCollection<string> LogMessages { get; } = new();
    public ObservableCollection<string> ChatMessages { get; } = new();
    public ObservableCollection<string> Participants { get; } = new();

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    public string ChannelName
    {
        get => _channelName;
        set => SetProperty(ref _channelName, value);
    }

    public string ChatInput
    {
        get => _chatInput;
        set => SetProperty(ref _chatInput, value);
    }

    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    public ICommand SendChatCommand { get; }
    public ICommand LeaveCommand { get; }

    public event EventHandler? LeaveRequested;

    public WarRoomViewModel(AgoraWarRoomClient agoraClient)
    {
        _agoraClient = agoraClient;

        SendChatCommand = new RelayCommand(
            execute: _ => OnSendChat(),
            canExecute: _ => !string.IsNullOrWhiteSpace(ChatInput)
        );

        LeaveCommand = new RelayCommand(
            execute: _ => OnLeave(),
            canExecute: _ => true
        );

        // Subscribe to Agora events
        _agoraClient.StateChanged += OnStateChanged;
        _agoraClient.ParticipantChanged += OnParticipantChanged;
        _agoraClient.LogReceived += OnLogReceived;
        _agoraClient.ChatMessageReceived += OnChatReceived;
    }

    public async Task JoinAsync(string channelName, string displayName)
    {
        ChannelName = channelName;
        DisplayName = displayName;
        var userId = Guid.NewGuid().ToString("N")[..8];

        await _agoraClient.JoinWarRoomAsync(channelName, userId, displayName);

        // Start demo log simulation
        _ = Task.Run(SimulateLogs);
    }

    private async Task SimulateLogs()
    {
        var random = new Random();
        var sampleLogs = new[]
        {
            ("Critical", "ALERT: Unauthorized SSH login detected from 10.0.0.55"),
            ("Error", "ERROR: Firewall rule bypass attempt on port 443"),
            ("Warning", "WARN: Unusual outbound traffic spike to 185.220.101.x (TOR Exit Node)"),
            ("Info", "INF: IDS signature update completed successfully"),
            ("Critical", "ALERT: Malware C2 beacon detected — hash: a3f2b8c1d9e..."),
            ("Warning", "WARN: Failed login attempt #47 for admin@corp-dc01"),
            ("Info", "INF: Forensic snapshot initiated for /var/log/auth.log"),
            ("Error", "ERROR: Memory dump failed on endpoint WS-PC-0042"),
            ("Info", "INF: Network segment 10.0.2.0/24 quarantined successfully"),
            ("Critical", "ALERT: Lateral movement detected — PsExec from DC01 to FS02"),
        };

        while (_agoraClient.CurrentState == SessionState.Connected)
        {
            await Task.Delay(random.Next(1500, 4000));

            var (severity, message) = sampleLogs[random.Next(sampleLogs.Length)];
            var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");
            var logLine = $"[{timestamp}] [{severity.ToUpper()}] {message}";

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                LogMessages.Add(logLine);

                // Keep log buffer manageable
                while (LogMessages.Count > 200)
                    LogMessages.RemoveAt(0);
            });
        }
    }

    private void OnStateChanged(object? sender, SessionStateChangedEventArgs e)
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            ConnectionStatus = e.NewState.ToString();
            LogMessages.Add($"[SYSTEM] State: {e.OldState} → {e.NewState} | {e.Message}");
        });
    }

    private void OnParticipantChanged(object? sender, ParticipantChangedEventArgs e)
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            if (e.Action == ParticipantAction.Joined)
                Participants.Add($"🟢 {e.DisplayName}");
            else
                Participants.Remove($"🟢 {e.DisplayName}");
        });
    }

    private void OnLogReceived(object? sender, LogReceivedEventArgs e)
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            var severityTag = e.Log.Severity.ToString().ToUpper();
            LogMessages.Add($"[{e.Log.Timestamp:HH:mm:ss}] [{severityTag}] {e.Log.Message}");
        });
    }

    private void OnChatReceived(object? sender, string message)
    {
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            ChatMessages.Add($"[{DisplayName}] {message}");
        });
    }

    private async void OnSendChat()
    {
        if (string.IsNullOrWhiteSpace(ChatInput)) return;

        var msg = ChatInput;
        ChatInput = string.Empty;

        ChatMessages.Add($"[{DisplayName}] {msg}");
        await _agoraClient.SendChatMessageAsync(msg);
    }

    private async void OnLeave()
    {
        await _agoraClient.LeaveWarRoomAsync();
        LeaveRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _agoraClient.StateChanged -= OnStateChanged;
        _agoraClient.ParticipantChanged -= OnParticipantChanged;
        _agoraClient.LogReceived -= OnLogReceived;
        _agoraClient.ChatMessageReceived -= OnChatReceived;
        _agoraClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
