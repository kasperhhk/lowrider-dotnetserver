namespace K;

public class WebSocketMessageCentral : IMessageCentral
{
  private readonly IEnumerable<IMessageHandler> _messageHandlers;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly IWebSocketSerializationStrategy _serializationStrategy;

  public WebSocketMessageCentral(IEnumerable<IMessageHandler> messageHandlers, IHostApplicationLifetime hostApplicationLifetime, IWebSocketSerializationStrategy serializationStrategy)
  {
    _messageHandlers = messageHandlers;
    _hostApplicationLifetime = hostApplicationLifetime;
    _serializationStrategy = serializationStrategy;
  }

  public void Deliver(IEnumerable<IClient> recipients, IOutboundMessage message)
  {
    if (recipients.Any(c => c is not IWebSocketClient))
      throw new Exception("Can't handle non-websocket clients in websocketcentral.");

    var webSocketClients = recipients.Cast<IWebSocketClient>();
    var cancellationToken = _hostApplicationLifetime.ApplicationStopping;

    Task.Run(async () =>
    {
      var payload = _serializationStrategy.Serialize(message, out var typeSegmentOwner, out var typeSegment);
      using (typeSegmentOwner)
      {
        var sendings = new List<Task>();
        foreach (var client in webSocketClients)
        {
          var sending = Task.Run(async () => {
            await client.WriteMessage(typeSegment, _serializationStrategy.MessageType, false, cancellationToken);
            await client.WriteMessage(payload, _serializationStrategy.MessageType, true, cancellationToken);
          });
          sendings.Add(sending);
        }

        await Task.WhenAll(sendings);
      }
    }, cancellationToken);
  }

  public void Deliver(IUser sender, IInboundMessage message)
  {
    foreach (var handler in _messageHandlers.Where(_ => _.CanHandle(message)))
    {
      Task.Run(() =>
      {
        handler.Handle(sender, message);
      }, _hostApplicationLifetime.ApplicationStopping);
    }
  }
}