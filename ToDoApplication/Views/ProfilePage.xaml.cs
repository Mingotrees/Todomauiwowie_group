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
        _ = LoadProfileAsync();
    }

    private async Task LoadProfileAsync()
    {
        UsernameLabel.Text = LocalAuthService.GetCurrentDisplayName();
        EmailLabel.Text = LocalAuthService.GetCurrentEmail();

        int userId = LocalAuthService.GetCurrentUserId();
        if (userId <= 0)
        {
            TotalTasksLabel.Text = "0";
            CompletedTasksLabel.Text = "0";
            return;
        }

        var activeItems = await TodoApiService.GetItemsAsync("active", userId);
        var completedItems = await TodoApiService.GetItemsAsync("inactive", userId);

        TotalTasksLabel.Text = (activeItems.Count + completedItems.Count).ToString();
        CompletedTasksLabel.Text = completedItems.Count.ToString();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            LocalAuthService.Logout();
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}