using System.Net.Http.Json;
using Messenger.Contracts.Dtos.Conversations;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Dtos.Users;
using Messenger.Contracts.Requests.Messages;

namespace Messenger.Client.Shared.Services;

public class ChatApi(HttpClient http)
{
    public async Task<List<ConversationListItemDto>> GetConversationsAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<ConversationListItemDto>>("api/conversations", ct) ?? [];

    public async Task<ConversationDto?> OpenDirectAsync(Guid userId, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/conversations/direct",
            new OpenDirectRequest(userId), ct);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ConversationDto>(ct)
            : null;
    }

    public async Task<ConversationDto?> GetConversationAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"api/conversations/{id}", ct);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ConversationDto>(ct)
            : null;
    }

    public async Task<List<MessageDto>> GetHistoryAsync(
        Guid conversationId, Guid? before = null, int limit = 50, CancellationToken ct = default)
    {
        var url = $"api/conversations/{conversationId}/messages?limit={limit}";
        if (before is not null) url += $"&before={before}";
        return await http.GetFromJsonAsync<List<MessageDto>>(url, ct) ?? [];
    }

    public async Task<MessageDto?> SendMessageAsync(
        Guid conversationId, string content, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(
            $"api/conversations/{conversationId}/messages",
            new SendMessageRequest(content), ct);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<MessageDto>(ct)
            : null;
    }

    public async Task MarkReadAsync(Guid conversationId, Guid messageId, CancellationToken ct = default)
        => await http.PostAsJsonAsync(
            $"api/conversations/{conversationId}/messages/read",
            new MarkReadRequest(messageId), ct);

    public async Task<List<UserDto>> SearchUsersAsync(string query, CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<UserDto>>(
            $"api/users/search?q={Uri.EscapeDataString(query)}", ct) ?? [];
}