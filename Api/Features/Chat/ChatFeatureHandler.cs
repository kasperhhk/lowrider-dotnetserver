using Api.Features.Chat.Handlers;
using Api.Messaging;
using Api.Messaging.Models;
using Api.Users;

namespace Api.Features.Chat;

public class ChatFeatureHandler : IFeatureHandler
{
    public const string FeatureName = "CHAT";
    public string Name => FeatureName;

    private readonly IServiceProvider _serviceProvider;

    public ChatFeatureHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Handle(IUser sender, IncomingPackage package, CancellationToken cancellationToken)
    {
        return package.Command switch
        {
            IncomingChatMessage.Command => Handle<IncomingChatMessageHandler, IncomingChatMessage>(sender, package, cancellationToken),
            _ => throw new MessageException("Unknown command")
        };

        Task Handle<THandler, TPayload>(IUser sender, IncomingPackage package, CancellationToken cancellationToken) where THandler : IChatHandler<TPayload>
        {
            var handler = _serviceProvider.GetRequiredService<THandler>();
            var payload = package.Payload.Deserialize<TPayload>();
            return handler.Handle(sender, payload, cancellationToken);
        }
    }
}

