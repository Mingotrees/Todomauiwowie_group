using ToDoApplication.Services;

namespace ToDoApplication.Views;

public partial class CompletedPage : ContentPage
{
    public CompletedPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CompletedCollection.ItemsSource = DummyData.TodoItems.Where(t => t.status == "Completed").ToList();
    }
}