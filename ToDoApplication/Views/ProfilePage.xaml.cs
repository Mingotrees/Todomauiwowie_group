using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var user = DummyData.CurrentUser;
        UsernameLabel.Text = user.username;
        EmailLabel.Text = user.email;
        TotalTasksLabel.Text = DummyData.TodoItems.Count.ToString();
        CompletedTasksLabel.Text = DummyData.TodoItems.Count(t => t.status
        == "Completed").ToString();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}