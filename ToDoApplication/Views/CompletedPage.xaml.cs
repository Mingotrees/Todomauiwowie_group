using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class CompletedPage : ContentPage
{
    public List<TodoItem> CompletedItems =>
        DummyData.TodoItems.Where(t => t.status == "Completed").ToList();

    public CompletedPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshList();
    }

    private void RefreshList()
    {
        CompletedCollection.ItemsSource = null;
        CompletedCollection.ItemsSource = CompletedItems;
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.CommandParameter is TodoItem item)
        {
            item.status = "Pending";
            RefreshList();
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton imageButton &&
            imageButton.CommandParameter is TodoItem item)
        {
            DummyData.TodoItems.Remove(item);
            RefreshList();
        }
    }
}
