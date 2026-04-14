using Microsoft.Maui.Storage;

namespace ToDoApplication.Services
{
    public static class LocalAuthService
    {
        private const string UserIdKey = "saved_user_id";
        private const string FirstNameKey = "saved_first_name";
        private const string LastNameKey = "saved_last_name";
        private const string EmailKey = "saved_email";

        public static void SaveSession(int userId, string firstName, string lastName, string email)
        {
            Preferences.Set(UserIdKey, userId);
            Preferences.Set(FirstNameKey, firstName);
            Preferences.Set(LastNameKey, lastName);
            Preferences.Set(EmailKey, email);
        }

        public static bool HasSession()
        {
            return Preferences.Get(UserIdKey, 0) > 0;
        }

        public static int GetCurrentUserId()
        {
            return Preferences.Get(UserIdKey, 0);
        }

        public static string GetCurrentEmail()
        {
            return Preferences.Get(EmailKey, string.Empty);
        }

        public static string GetCurrentDisplayName()
        {
            string firstName = Preferences.Get(FirstNameKey, string.Empty);
            string lastName = Preferences.Get(LastNameKey, string.Empty);
            return string.Join(" ", new[] { firstName, lastName }.Where(value => !string.IsNullOrWhiteSpace(value))).Trim();
        }

        public static void Logout()
        {
            Preferences.Remove(UserIdKey);
            Preferences.Remove(FirstNameKey);
            Preferences.Remove(LastNameKey);
            Preferences.Remove(EmailKey);
        }
    }
}

