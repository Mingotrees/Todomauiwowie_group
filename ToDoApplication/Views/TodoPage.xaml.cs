using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class TodoPage : ContentPage
{
    public List<TodoItem> TodoItems =>
        DummyData.TodoItems.Where(t => t.status != "Completed").ToList();

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
            item.status = e.Value ? "Completed" : "Pending";
            OnAppearing();
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton imageButton &&
            imageButton.CommandParameter is TodoItem item)
        {
            DummyData.TodoItems.Remove(item);
            OnAppearing();
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

        DummyData.TodoItems.Add(new TodoItem
        {
            item_id = DummyData.TodoItems.Count + 1,
            item_name = title,
            item_description = description,
            status = "Pending",
            user_id = 1
        });

        OnAppearing();
    }
}
