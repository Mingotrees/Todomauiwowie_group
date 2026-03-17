using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class SignupPage : ContentPage
{
    public SignupPage()
    {
        InitializeComponent();
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text ?? "";
        string email = EmailEntry.Text ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter username, email, and password.", "OK");
            return;
        }

        await LocalAuthService.SaveUserAsync(username, email, password);
        await DisplayAlert("Success", "Account created successfully.", "OK");

        Application.Current!.MainPage = new NavigationPage(new LoginPage());
    }

    private async void GoToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}
