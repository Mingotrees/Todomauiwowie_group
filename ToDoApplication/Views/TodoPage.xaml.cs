using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class TodoPage : ContentPage
{
    private List<TodoItem> _todoItems = new();

    public List<TodoItem> TodoItems => _todoItems;

    public TodoPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshTodosAsync();
    }

    private async Task RefreshTodosAsync()
    {
        int userId = LocalAuthService.GetCurrentUserId();
        if (userId <= 0)
        {
            _todoItems = new List<TodoItem>();
            TodosCollection.ItemsSource = _todoItems;
            return;
        }

        _todoItems = await TodoApiService.GetItemsAsync("active", userId);
        TodosCollection.ItemsSource = null;
        TodosCollection.ItemsSource = TodoItems;
    }

    private async void OnItemCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is TodoItem item)
        {
            var result = await TodoApiService.ChangeItemStatusAsync(item.item_id, e.Value);
            if (!result.IsSuccess)
            {
                await DisplayAlert("Error", result.Message, "OK");
                return;
            }

            await RefreshTodosAsync();
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

            await RefreshTodosAsync();
        }
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        string title = await DisplayPromptAsync("New Task", "Enter task title:");

        if (string.IsNullOrWhiteSpace(title))
            return;

        string description = await DisplayPromptAsync("New Task", "Enter task description:");

        if (string.IsNullOrWhiteSpace(description))
            description = "No description";

        int userId = LocalAuthService.GetCurrentUserId();
        if (userId <= 0)
        {
            await DisplayAlert("Error", "No active session. Please log in again.", "OK");
            return;
        }

        var result = await TodoApiService.AddItemAsync(title, description, userId);
        if (!result.IsSuccess)
        {
            await DisplayAlert("Error", result.Message, "OK");
            return;
        }

        await RefreshTodosAsync();
    }
}
