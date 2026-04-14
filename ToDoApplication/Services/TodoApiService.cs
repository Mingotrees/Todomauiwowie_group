using System.Text.Json;
using System.Text;
using ToDoApplication.Models;

namespace ToDoApplication.Services;

public static class TodoApiService
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://todo-list.dcism.org/")
    };

    public static async Task<List<TodoItem>> GetItemsAsync(string apiStatus, int userId)
    {
        try
        {
            string route = $"getItems_action.php?status={Uri.EscapeDataString(apiStatus)}&user_id={userId}";
            using var response = await HttpClient.GetAsync(route);
            string body = await response.Content.ReadAsStringAsync();

            if (!TryParseJson(body, out JsonDocument? doc))
                return new List<TodoItem>();

            using (doc)
            {
                JsonElement root = doc.RootElement;

                int status = ReadInt(root, "status");
                if (status != 200 || !root.TryGetProperty("data", out JsonElement data) || data.ValueKind != JsonValueKind.Object)
                    return new List<TodoItem>();

                var items = new List<TodoItem>();
                foreach (JsonProperty entry in data.EnumerateObject())
                {
                    JsonElement itemNode = entry.Value;
                    if (itemNode.ValueKind != JsonValueKind.Object)
                        continue;

                    items.Add(new TodoItem
                    {
                        item_id = ReadInt(itemNode, "item_id"),
                        item_name = ReadString(itemNode, "item_name"),
                        item_description = ReadString(itemNode, "item_description"),
                        status = MapApiStatusToLocal(ReadString(itemNode, "status")),
                        user_id = ReadInt(itemNode, "user_id")
                    });
                }

                return items;
            }
        }
        catch (HttpRequestException)
        {
            return new List<TodoItem>();
        }
        catch (TaskCanceledException)
        {
            return new List<TodoItem>();
        }
    }

    public static async Task<(bool IsSuccess, string Message)> AddItemAsync(string itemName, string itemDescription, int userId)
    {
        try
        {
            var payload = new
            {
                item_name = itemName,
                item_description = itemDescription,
                user_id = userId
            };

            string jsonBody = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            using var response = await HttpClient.PostAsync("addItem_action.php", content);
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

    public static async Task<(bool IsSuccess, string Message)> UpdateItemAsync(TodoItem item)
    {
        try
        {
            var payload = new
            {
                item_name = item.item_name,
                item_description = item.item_description,
                item_id = item.item_id
            };

            string jsonBody = JsonSerializer.Serialize(payload);
            using var request = new HttpRequestMessage(HttpMethod.Put, "editItem_action.php")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            using var response = await HttpClient.SendAsync(request);
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

    public static async Task<(bool IsSuccess, string Message)> ChangeItemStatusAsync(int itemId, bool isCompleted)
    {
        try
        {
            var payload = new
            {
                status = isCompleted ? "inactive" : "active",
                item_id = itemId
            };

            string jsonBody = JsonSerializer.Serialize(payload);
            using var request = new HttpRequestMessage(HttpMethod.Put, "statusItem_action.php")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            using var response = await HttpClient.SendAsync(request);
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

    public static async Task<(bool IsSuccess, string Message)> DeleteItemAsync(int itemId)
    {
        try
        {
            string route = $"deleteItem_action.php?item_id={itemId}";
            using var response = await HttpClient.DeleteAsync(route);
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

    private static string MapApiStatusToLocal(string apiStatus)
    {
        return apiStatus.Equals("inactive", StringComparison.OrdinalIgnoreCase) ? "Completed" : "Pending";
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
                return (true, string.IsNullOrWhiteSpace(message) ? "Success" : message.Trim());

            return (false, string.IsNullOrWhiteSpace(message) ? "Request failed." : message.Trim());
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

