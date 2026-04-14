using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class CompletedPage : ContentPage
{
    private List<TodoItem> _completedItems = new();

    public List<TodoItem> CompletedItems => _completedItems;

    public CompletedPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshListAsync();
    }

    private async Task RefreshListAsync()
    {
        int userId = LocalAuthService.GetCurrentUserId();
        if (userId > 0)
            _completedItems = await TodoApiService.GetItemsAsync("inactive", userId);
        else
            _completedItems = new List<TodoItem>();

        CompletedCollection.ItemsSource = null;
        CompletedCollection.ItemsSource = CompletedItems;
    }

    private async void OnUndoClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.CommandParameter is TodoItem item)
        {
            var result = await TodoApiService.ChangeItemStatusAsync(item.item_id, false);
            if (!result.IsSuccess)
            {
                await DisplayAlert("Error", result.Message, "OK");
                return;
            }

            await RefreshListAsync();
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton imageButton &&
            imageButton.CommandParameter is TodoItem item)
        {
            var result = await TodoApiService.DeleteItemAsync(item.item_id);
            if (!result.IsSuccess)
            {
                await DisplayAlert("Error", result.Message, "OK");
                return;
            }

            await RefreshListAsync();
        }
    }
}
