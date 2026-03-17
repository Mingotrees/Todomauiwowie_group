using ToDoApplication.Models;

namespace ToDoApplication.Services
{
    public static class DummyData
    {
        public static User CurrentUser { get; set; } = new User
        {
            username = "",
            email = "",
            password = ""
        };

        public static List<TodoItem> TodoItems { get; set; } = new();
    }
}
