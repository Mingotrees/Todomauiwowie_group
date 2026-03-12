using ToDoApplication.Services;

namespace ToDoApplication.Views;
using ToDoApplication.Services;
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        
        if (email == DummyData.CurrentUser.email && password == DummyData.CurrentUser.password)
        {
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Invalid email or password", "OK");
        }
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignupPage());
    }
}