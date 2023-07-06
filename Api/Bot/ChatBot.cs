using Api.Client.InMemory;
using Api.Features.Chat;
using Api.Features.Chat.Handlers;
using Api.Messaging;
using Api.Messaging.Models;
using Api.Users;

namespace Api.Bot;

public class ChatBot
{
    private readonly IInMemoryClientFactory _clientFactory;
    private readonly IMessageBroker _messageBroker;

    public User User { get; }

    public ChatBot(IInMemoryClientFactory clientFactory, IMessageBroker messageBroker, User user)
    {
        _clientFactory = clientFactory;
        _messageBroker = messageBroker;
        User = user;
    }

    public async Task ProcessMessages(CancellationToken cancellationToken)
    {
        using var registeredClient = _clientFactory.CreateAndRegister(User);

        await foreach (var message in registeredClient.Client.ReadAllMessages(cancellationToken))
        {
            await HandleMessage(message);
        }
    }

    private Task HandleMessage(InMemoryOutgoingPackage message)
    {
        return message.Payload switch
        {
            OutboundChatMessage oMsg => HandleOutboundChatMessage(oMsg),
            _ => Task.CompletedTask
        };
    }

    private Task HandleOutboundChatMessage(OutboundChatMessage oMsg)
    {
        if (oMsg.user == User.Id)
            return Task.CompletedTask; // Ignore own messages

        var response = PassThroughIncomingPayload.From(new IncomingChatMessage("wow! that's so interesting!"));
        _messageBroker.Send(User, new IncomingPackage(ChatFeature.Name, IncomingChatMessage.Command, response));

        return Task.CompletedTask;
    }
}