using Mapster;
using Messenger.Contracts.Dtos.Conversations;
using Messenger.Contracts.Dtos.Users;
using Messenger.Domain;

namespace Messenger.Api.BusinessLogic.Mappings;

public class MappingRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Conversation, ConversationDto>()
            .Map(dest => dest.Participants,
                src => src.Participants.Select(p =>
                    new UserDto(p.User.Id, p.User.Username, p.User.DisplayName, p.User.CreatedAt)));
    }
}