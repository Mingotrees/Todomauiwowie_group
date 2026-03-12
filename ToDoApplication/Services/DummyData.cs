using ToDoApplication.Models;
namespace ToDoApplication.Services
{
    public static class DummyData
    {
        public static User CurrentUser { get; set; } = new User
        {
            username = "John Doe",
            email = "john@example.com",
            password = "password123"
        };

        public static List<TodoItem> TodoItems { get; set; } = new()
        {
            new TodoItem { item_id = 1, item_name = "Buy groceries",      item_description = "Milk, eggs, bread",               status = "Pending",   user_id = 1 },
            new TodoItem { item_id = 2, item_name = "Read a book",        item_description = "Finish reading Clean Code",       status = "Pending",   user_id = 1 },
            new TodoItem { item_id = 3, item_name = "Exercise",           item_description = "30 min cardio",                   status = "Completed", user_id = 1 },
            new TodoItem { item_id = 4, item_name = "Pay bills",          item_description = "Electricity and internet",        status = "Completed", user_id = 1 },
            new TodoItem { item_id = 5, item_name = "Doctor appointment", item_description = "Annual checkup",                  status = "Pending",   user_id = 1 }
        };
    }
}