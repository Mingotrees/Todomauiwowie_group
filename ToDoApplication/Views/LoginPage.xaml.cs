using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string usernameOrEmail = UsernameOrEmailEntry.Text ?? "";
        string password = PasswordEntry.Text ?? "";

        bool isValid = await LocalAuthService.LoginAsync(usernameOrEmail, password);

        if (isValid)
        {
            await DisplayAlert("Success", "Login successful.", "OK");
            Application.Current!.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Invalid username/email or password.", "OK");
        }
    }

    private async void GoToSignupClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignupPage());
    }
}
