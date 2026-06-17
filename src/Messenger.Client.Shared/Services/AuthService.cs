using System.Net.Http.Json;
using Messenger.Contracts;
using Messenger.Contracts.Dtos.Users;
using Messenger.Contracts.Requests.Auth;
using Messenger.Contracts.Responses.Auth;

namespace Messenger.Client.Shared.Services;

public class AuthService(HttpClient http, ITokenStorage storage)
{
    public UserDto? CurrentUser { get; private set; }

    public event Action? AuthStateChanged;

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await http.PostAsJsonAsync("api/auth/login",
            new LoginRequest(username, password));
        if (!response.IsSuccessStatusCode) return false;

        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (payload is null) return false;

        await storage.SaveTokensAsync(payload.AccessToken, payload.RefreshToken);
        CurrentUser = payload.User;
        AuthStateChanged?.Invoke();
        return true;
    }

    public async Task<(bool Ok, string? Error)> RegisterAsync(string username, string displayName, string password)
    {
        var response = await http.PostAsJsonAsync("api/auth/register",
            new RegisterRequest(username, displayName, password));

        if (response.IsSuccessStatusCode)
        {
            var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (payload is null) return (false, "Некорректный ответ сервера");
            await storage.SaveTokensAsync(payload.AccessToken, payload.RefreshToken);
            CurrentUser = payload.User;
            AuthStateChanged?.Invoke();
            return (true, null);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            return (false, "Имя пользователя занято");

        return (false, "Не удалось зарегистрироваться");
    }

    public async Task LogoutAsync()
    {
        var refresh = await storage.GetRefreshTokenAsync();
        if (refresh is not null)
        {
            try
            {
                await http.PostAsJsonAsync("api/auth/logout", new RefreshRequest(refresh));
            }
            catch (HttpRequestException)
            {
                
            }
        }
        await storage.ClearAsync();
        CurrentUser = null;
        AuthStateChanged?.Invoke();
    }
}