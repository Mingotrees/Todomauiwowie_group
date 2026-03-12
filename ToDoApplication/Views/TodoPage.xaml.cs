using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class TodoPage : ContentPage
{
    public List<TodoItem> TodoItems => DummyData.TodoItems.Where(t => t.status.ToString() != "Completed").ToList();

    public TodoPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TodosCollection.ItemsSource = null;
        TodosCollection.ItemsSource = TodoItems;
    }

    private void OnItemCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is TodoItem item)
        {
            item.status = e.Value.ToString();
            OnAppearing();
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is TodoItem item)
        {
            bool confirm = await DisplayAlert("Delete", $"Delete '{item.item_name}'?", "Yes", "No");
            if (confirm)
            {
                DummyData.TodoItems.Remove(item);
                OnAppearing();
            }
        }
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        string title = await DisplayPromptAsync("New Task", "Enter task title:");
        if (!string.IsNullOrWhiteSpace(title))
        {
            DummyData.TodoItems.Add(new TodoItem
            {
                item_id = DummyData.TodoItems.Count + 1,
                item_name = title,
                item_description = "No description",
                status = "Pending",
            });
            OnAppearing();
        }
    }
}