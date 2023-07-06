namespace Api.Messaging.Models;

public record OutgoingPackage<TPayload>(string Feature, string Command, TPayload Payload) where TPayload : notnull;
public record IncomingPackage(string Feature, string Command, IIncomingPayload Payload);
public interface IIncomingPayload
{
    T Deserialize<T>();
}
