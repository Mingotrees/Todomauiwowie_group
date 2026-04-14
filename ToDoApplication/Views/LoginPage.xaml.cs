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
        string email = UsernameOrEmailEntry.Text ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter email and password.", "OK");
            return;
        }

        var result = await AuthApiService.SignInAsync(email, password);

        if (result.IsSuccess)
        {
            LocalAuthService.SaveSession(result.UserId, result.FirstName, result.LastName, result.Email);

            await DisplayAlert("Success", "Login successful.", "OK");
            Application.Current!.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", result.Message, "OK");
        }
    }

    private async void GoToSignupClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignupPage());
    }
}
