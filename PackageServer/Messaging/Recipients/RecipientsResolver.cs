using Api.Client;

namespace Api.Messaging.Recipients;

public class RecipientsResolver : IRecipientsResolver
{
    private readonly IClientManager _clientManager;

    public RecipientsResolver(IClientManager clientManager)
    {
        _clientManager = clientManager;
    }

    public Recipients Everybody()
    {
        return new Recipients(_clientManager.Clients);
    }
}
