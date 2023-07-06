using Api.Client;

namespace Api.Messaging.Recipients;

public record Recipients(IEnumerable<IClient> Clients);
