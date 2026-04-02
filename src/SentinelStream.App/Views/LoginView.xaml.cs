using System.Windows.Controls;
using SentinelStream.App.ViewModels;

namespace SentinelStream.App.Views;

public partial class LoginView : UserControl
{
    public LoginViewModel ViewModel => (LoginViewModel)DataContext;

    public LoginView()
    {
        InitializeComponent();
    }
}
