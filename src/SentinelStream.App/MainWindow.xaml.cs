using System.Windows;
using SentinelStream.App.ViewModels;
using SentinelStream.App.Views;
using SentinelStream.Core.Agora;

namespace SentinelStream.App;

public partial class MainWindow : Window
{
    private const string DefaultAppId = "sentinel_stream_demo";

    public MainWindow()
    {
        InitializeComponent();
        ShowLogin();
    }

    private void ShowLogin()
    {
        var loginView = new LoginView();
        var loginVm = loginView.ViewModel;

        loginVm.JoinRequested += async (_, args) =>
        {
            try
            {
                // Load config (use demo AppId if .env not configured)
                string appId = DefaultAppId;
                try
                {
                    var config = Services.ConfigLoader.LoadFromEnvFile();
                    if (!string.IsNullOrWhiteSpace(config.AgoraAppId) &&
                        config.AgoraAppId != "your_agora_app_id_here")
                    {
                        appId = config.AgoraAppId;
                    }
                }
                catch
                {
                    // Use default if .env not available
                }

                var agoraClient = new AgoraWarRoomClient(appId);
                var warRoomVm = new WarRoomViewModel(agoraClient);

                warRoomVm.LeaveRequested += (_, _) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        warRoomVm.Dispose();
                        ShowLogin();
                    });
                };

                var warRoomView = new WarRoomView { DataContext = warRoomVm };
                MainContent.Content = warRoomView;

                await warRoomVm.JoinAsync(args.WarRoomId, args.DisplayName);
            }
            catch (Exception ex)
            {
                loginVm.StatusMessage = $"Error: {ex.Message}";
                loginVm.IsConnecting = false;
            }
        };

        MainContent.Content = loginView;
    }
}