using ToDoApplication.Services;
using ToDoApplication.Views;

namespace ToDoApplication;
// Wanako kasabot

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        if (LocalAuthService.HasAccount())
            MainPage = new NavigationPage(new LoginPage());
        else
            MainPage = new NavigationPage(new SignupPage());
    }
}
