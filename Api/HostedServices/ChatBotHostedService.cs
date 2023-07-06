using Api.Bot;
using Api.Client.InMemory;
using Api.Messaging;
using Api.Users;

namespace Api.HostedServices;

public class ChatBotHostedService : BackgroundService
{
    private readonly IInMemoryClientFactory _inMemoryClientFactory;
    private readonly IMessageBroker _messageBroker;

    public ChatBotHostedService(IInMemoryClientFactory inMemoryClientFactory, IMessageBroker messageBroker)
    {
        _inMemoryClientFactory = inMemoryClientFactory;
        _messageBroker = messageBroker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var user = new User("bot_Eager");
        var bot = new ChatBot(_inMemoryClientFactory, _messageBroker, user);

        await bot.ProcessMessages(stoppingToken);
    }
}