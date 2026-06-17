using Mapster;
using Messenger.Api.BusinessLogic.Mappings;
using Messenger.Api.BusinessLogic.Services;
using Messenger.Contracts.Api.BusinessLogic;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.Api.BusinessLogic;

public static class DependencyInjections
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IConversationsService, ConversationsService>();

        TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingRegister).Assembly);
        
        return services;
    }
}