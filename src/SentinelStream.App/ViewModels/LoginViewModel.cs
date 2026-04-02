using System.Windows.Input;

namespace SentinelStream.App.ViewModels;

/// <summary>
/// ViewModel for the Login screen where the user enters their name and War Room ID.
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private string _displayName = string.Empty;
    private string _warRoomId = string.Empty;
    private string _statusMessage = "Ready to connect.";
    private bool _isConnecting;

    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    public string WarRoomId
    {
        get => _warRoomId;
        set => SetProperty(ref _warRoomId, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsConnecting
    {
        get => _isConnecting;
        set => SetProperty(ref _isConnecting, value);
    }

    public ICommand JoinCommand { get; }

    /// <summary>
    /// Event raised when the user successfully joins a War Room.
    /// The MainWindow listens to this to navigate to the WarRoom view.
    /// </summary>
    public event EventHandler<(string DisplayName, string WarRoomId)>? JoinRequested;

    public LoginViewModel()
    {
        JoinCommand = new RelayCommand(
            execute: _ => OnJoin(),
            canExecute: _ => !string.IsNullOrWhiteSpace(DisplayName)
                          && !string.IsNullOrWhiteSpace(WarRoomId)
                          && !IsConnecting
        );
    }

    private void OnJoin()
    {
        IsConnecting = true;
        StatusMessage = $"Joining War Room: {WarRoomId}...";
        JoinRequested?.Invoke(this, (DisplayName, WarRoomId));
    }
}
