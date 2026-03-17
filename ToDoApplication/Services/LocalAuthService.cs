using Microsoft.Maui.Storage;

namespace ToDoApplication.Services
{
    public static class LocalAuthService
    {
        private const string UsernameKey = "saved_username";
        private const string EmailKey = "saved_email";
        private const string PasswordKey = "saved_password";

        public static async Task SaveUserAsync(string username, string email, string password)
        {
            Preferences.Set(UsernameKey, username);
            Preferences.Set(EmailKey, email);
            await SecureStorage.Default.SetAsync(PasswordKey, password);
        }

        public static async Task<bool> LoginAsync(string username, string password)
        {
            string savedUsername = Preferences.Get(UsernameKey, string.Empty);
            string savedPassword = await SecureStorage.Default.GetAsync(PasswordKey) ?? string.Empty;

            return username == savedUsername && password == savedPassword;
        }

        public static string GetSavedUsername()
        {
            return Preferences.Get(UsernameKey, string.Empty);
        }

        public static string GetSavedEmail()
        {
            return Preferences.Get(EmailKey, string.Empty);
        }

        public static bool HasAccount()
        {
            return !string.IsNullOrWhiteSpace(Preferences.Get(UsernameKey, string.Empty));
        }

        public static void Logout()
        {
            Preferences.Remove(UsernameKey);
            Preferences.Remove(EmailKey);
            SecureStorage.Default.Remove(PasswordKey);
        }
    }
}
