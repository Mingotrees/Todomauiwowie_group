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
        string firstName = FirstNameEntry.Text ?? "";
        string lastName = LastNameEntry.Text ?? "";
        string email = EmailEntry.Text ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }

        if (!password.Equals(confirmPassword, StringComparison.Ordinal))
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        var result = await AuthApiService.SignUpAsync(firstName, lastName, email, password, confirmPassword);
        if (!result.IsSuccess)
        {
            await DisplayAlert("Error", result.Message, "OK");
            return;
        }

        await DisplayAlert("Success", result.Message, "OK");
        Application.Current!.MainPage = new NavigationPage(new LoginPage());
    }

    private async void GoToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}
