using Messenger.Contracts;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Hubs.Events;
using Messenger.Contracts.Hubs.Interfaces;
using Messenger.Contracts.Requests.Messages;
using Microsoft.AspNetCore.SignalR.Client;

namespace Messenger.Client.Shared.Services;

public class ChatConnection(ITokenStorage storage) : IAsyncDisposable
{
    private HubConnection? _hub;

    public event Action<MessageDto>? MessageReceived;
    public event Action<TypingEvent>? UserTyping;
    public event Action<MessageReadEvent>? MessageRead;
    public event Action<HubConnectionState>? StateChanged;

    public HubConnectionState State => _hub?.State ?? HubConnectionState.Disconnected;

    public async Task StartAsync(string apiBaseUrl)
    {
        if (_hub is not null) return;

        _hub = new HubConnectionBuilder()
            .WithUrl($"{apiBaseUrl}/hubs/chat", options =>
            {
                options.AccessTokenProvider = async () => await storage.GetAccessTokenAsync();
            })
            .WithAutomaticReconnect()
            .Build();

        _hub.On<MessageDto>(nameof(IChatClient.MessageReceived),
            msg => MessageReceived?.Invoke(msg));

        _hub.On<TypingEvent>(nameof(IChatClient.UserTyping),
            evt => UserTyping?.Invoke(evt));

        _hub.On<MessageReadEvent>(nameof(IChatClient.MessageRead),
            evt => MessageRead?.Invoke(evt));

        _hub.Reconnecting += _ => { StateChanged?.Invoke(HubConnectionState.Reconnecting); return Task.CompletedTask; };
        _hub.Reconnected += _ => { StateChanged?.Invoke(HubConnectionState.Connected); return Task.CompletedTask; };
        _hub.Closed += _ => { StateChanged?.Invoke(HubConnectionState.Disconnected); return Task.CompletedTask; };

        await _hub.StartAsync();
        StateChanged?.Invoke(HubConnectionState.Connected);
    }

    public async Task<MessageDto?> SendMessageAsync(Guid conversationId, string content)
    {
        if (_hub is null || _hub.State != HubConnectionState.Connected) return null;
        return await _hub.InvokeAsync<MessageDto>(
            nameof(IChatHub.SendMessage),
            new SendMessageHubRequest(conversationId, content));
    }

    public async Task SendTypingAsync(Guid conversationId, bool isTyping)
    {
        if (_hub is null || _hub.State != HubConnectionState.Connected) return;
        await _hub.SendAsync(nameof(IChatHub.Typing),
            new TypingHubRequest(conversationId, isTyping));
    }

    public async Task MarkReadAsync(Guid conversationId, Guid messageId)
    {
        if (_hub is null || _hub.State != HubConnectionState.Connected) return;
        await _hub.SendAsync(nameof(IChatHub.MarkRead),
            new MarkReadHubRequest(conversationId, messageId));
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub is not null)
        {
            await _hub.DisposeAsync();
            _hub = null;
        }
    }
}