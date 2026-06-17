using Messenger.Client.Shared;
using Messenger.Client.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<ITokenStorage, LocalStorageTokenStorage>();
builder.Services.AddTransient<AuthorizationHandler>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
    throw new InvalidOperationException("ApiBaseUrl is not configured");

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ChatApi>();
builder.Services.AddSingleton<ChatConnection>();
builder.Services.AddSingleton(new ApiConfig(apiBaseUrl));

await builder.Build().RunAsync();