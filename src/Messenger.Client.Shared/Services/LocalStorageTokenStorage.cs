using Microsoft.JSInterop;

namespace Messenger.Client.Shared.Services;

public class LocalStorageTokenStorage(IJSRuntime js) : ITokenStorage
{
    private const string AccessKey = "messenger.accessToken";
    private const string RefreshKey = "messenger.refreshToken";

    public async Task<string?> GetAccessTokenAsync()
        => await js.InvokeAsync<string?>("localStorage.getItem", AccessKey);

    public async Task<string?> GetRefreshTokenAsync()
        => await js.InvokeAsync<string?>("localStorage.getItem", RefreshKey);

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await js.InvokeVoidAsync("localStorage.setItem", AccessKey, accessToken);
        await js.InvokeVoidAsync("localStorage.setItem", RefreshKey, refreshToken);
    }

    public async Task ClearAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", AccessKey);
        await js.InvokeVoidAsync("localStorage.removeItem", RefreshKey);
    }
}