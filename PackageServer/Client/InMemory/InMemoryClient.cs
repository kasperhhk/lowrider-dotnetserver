using Api.Users;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Api.Client.InMemory;

public class InMemoryClient : ISendableClient<InMemoryOutgoingPackage>
{
    private readonly Channel<InMemoryOutgoingPackage> _messageChannel = Channel.CreateUnbounded<InMemoryOutgoingPackage>();
    private readonly ILogger<InMemoryClient> _logger;

    public string Id { get; } = Guid.NewGuid().ToString("D");
    public IUser User { get; }

    public InMemoryClient(IUser user, ILogger<InMemoryClient> logger)
    {
        User = user;
        _logger = logger;
    }

    public async Task Send(InMemoryOutgoingPackage data, CancellationToken cancellationToken)
    {
        try
        {
            await _messageChannel.Writer.WriteAsync(data, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing to inmemory channel for user {User}", User.Id);
        }
    }

    public IAsyncEnumerable<InMemoryOutgoingPackage> ReadAllMessages(CancellationToken cancellationToken)
    {
        return _messageChannel.Reader.ReadAllAsync(cancellationToken);
    }
}
