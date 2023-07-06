namespace Api.Messaging.Models;

public record OutgoingPackage<TPayload>(string Feature, string Command, TPayload Payload) where TPayload : notnull;
public record IncomingPackage(string Feature, string Command, IIncomingPayload Payload);
public interface IIncomingPayload
{
    T Deserialize<T>();
}

public record PassThroughIncomingPayload(object Payload) : IIncomingPayload
{
    public T Deserialize<T>()
    {
        return (T)Payload;
    }

    public static PassThroughIncomingPayload From<T>(T payload) where T : notnull
    {
        return new PassThroughIncomingPayload(payload);
    }
}
