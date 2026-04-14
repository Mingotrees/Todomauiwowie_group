using ToDoApplication.Services;
using ToDoApplication.Views;

namespace ToDoApplication;
// Wanako kasabot

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = LocalAuthService.HasSession()
            ? new AppShell()
            : new NavigationPage(new LoginPage());
    }
}
