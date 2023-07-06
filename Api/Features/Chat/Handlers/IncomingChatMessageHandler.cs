﻿using Api.Features.Chat;
using Api.Messaging;
using Api.Messaging.Recipients;
using Api.Users;

namespace Api.Features.Chat.Handlers;

public record IncomingChatMessage(string message)
{
    public const string Command = "CHATMSG";
}

public record OutboundChatMessage(string user, string message)
{
    public const string Command = "CHATMSG";
}

public class IncomingChatMessageHandler : IChatHandler<IncomingChatMessage>
{
    private readonly MessageBroker _messageBroker;
    private readonly IRecipientsResolver _recipientsResolver;

    public IncomingChatMessageHandler(MessageBroker messageBroker, IRecipientsResolver recipientsResolver)
    {
        _messageBroker = messageBroker;
        _recipientsResolver = recipientsResolver;
    }

    public Task Handle(IUser sender, IncomingChatMessage payload, CancellationToken cancellationToken)
    {
        var outgoingPackage = new OutgoingPackage<OutboundChatMessage>(
            ChatFeatureHandler.FeatureName,
            OutboundChatMessage.Command,
            new OutboundChatMessage(sender.Id, payload.message));

        var recipients = _recipientsResolver.Everybody();

        _messageBroker.Send(recipients, outgoingPackage);

        return Task.CompletedTask;
    }
}