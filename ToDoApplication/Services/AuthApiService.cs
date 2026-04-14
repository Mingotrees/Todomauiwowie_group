using System.Text.Json;
using System.Text;

namespace ToDoApplication.Services;

public static class AuthApiService
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://todo-list.dcism.org/")
    };

    public static async Task<(bool IsSuccess, string Message)> SignUpAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string confirmPassword)
    {
        try
        {
            var payload = new
            {
                first_name = firstName,
                last_name = lastName,
                email,
                password,
                confirm_password = confirmPassword
            };

            string jsonBody = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            using var response = await HttpClient.PostAsync("signup_action.php", content);
            string body = await response.Content.ReadAsStringAsync();
            return ParseSimpleStatusResponse(response.StatusCode, body);
        }
        catch (HttpRequestException)
        {
            return (false, "Unable to reach server. Please check your internet connection.");
        }
        catch (TaskCanceledException)
        {
            return (false, "Request timed out. Please try again.");
        }
    }

    public static async Task<(bool IsSuccess, int UserId, string FirstName, string LastName, string Email, string Message)> SignInAsync(
        string email,
        string password)
    {
        try
        {
            string route = $"signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";

            using var response = await HttpClient.GetAsync(route);
            string body = await response.Content.ReadAsStringAsync();

            if (!TryParseJson(body, out JsonDocument? doc))
                return (false, 0, string.Empty, string.Empty, string.Empty, BuildNonJsonMessage(response.StatusCode, body));

            using (doc)
            {
                JsonElement root = doc.RootElement;

                int status = ReadInt(root, "status");
                string message = ReadString(root, "message");

                if (status != 200)
                    return (false, 0, string.Empty, string.Empty, string.Empty, string.IsNullOrWhiteSpace(message) ? "Login failed." : message);

                if (!root.TryGetProperty("data", out JsonElement data))
                    return (false, 0, string.Empty, string.Empty, string.Empty, "Missing user data in response.");

                int userId = ReadInt(data, "id");
                string firstName = ReadString(data, "fname");
                string lastName = ReadString(data, "lname");
                string userEmail = ReadString(data, "email");

                return (true, userId, firstName, lastName, userEmail, string.IsNullOrWhiteSpace(message) ? "Success" : message);
            }
        }
        catch (HttpRequestException)
        {
            return (false, 0, string.Empty, string.Empty, string.Empty, "Unable to reach server. Please check your internet connection.");
        }
        catch (TaskCanceledException)
        {
            return (false, 0, string.Empty, string.Empty, string.Empty, "Request timed out. Please try again.");
        }
    }

    private static (bool IsSuccess, string Message) ParseSimpleStatusResponse(System.Net.HttpStatusCode statusCode, string body)
    {
        if (!TryParseJson(body, out JsonDocument? doc))
            return (false, BuildNonJsonMessage(statusCode, body));

        using (doc)
        {
            JsonElement root = doc.RootElement;

            int status = ReadInt(root, "status");
            string message = ReadString(root, "message");

            if (status == 200)
                return (true, string.IsNullOrWhiteSpace(message) ? "Success" : message);

            return (false, string.IsNullOrWhiteSpace(message) ? "Request failed." : message);
        }
    }

    private static bool TryParseJson(string body, out JsonDocument? document)
    {
        document = null;

        if (string.IsNullOrWhiteSpace(body))
            return false;

        try
        {
            document = JsonDocument.Parse(body);
            return true;
        }
        catch (JsonException)
        {
            int jsonStart = body.IndexOf('{');
            int jsonEnd = body.LastIndexOf('}');
            if (jsonStart < 0 || jsonEnd <= jsonStart)
                return false;

            string jsonCandidate = body.Substring(jsonStart, (jsonEnd - jsonStart) + 1);
            try
            {
                document = JsonDocument.Parse(jsonCandidate);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }

    private static string BuildNonJsonMessage(System.Net.HttpStatusCode statusCode, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return $"Server returned an empty response ({(int)statusCode}).";

        string trimmed = body.TrimStart();
        if (trimmed.StartsWith("<", StringComparison.Ordinal))
            return $"Server returned HTML instead of JSON ({(int)statusCode}). Please try again later.";

        return $"Unexpected server response ({(int)statusCode}).";
    }

    private static int ReadInt(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value))
            return 0;

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int numberValue))
            return numberValue;

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out int stringValue))
            return stringValue;

        return 0;
    }

    private static string ReadString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value))
            return string.Empty;

        if (value.ValueKind == JsonValueKind.String)
            return value.GetString() ?? string.Empty;

        return value.ToString();
    }
}

