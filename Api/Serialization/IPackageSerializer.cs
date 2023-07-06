using Api.Messaging;

namespace Api.Serialization;

public interface IPackageSerializer
{
    byte[] Serialize<TPayload>(OutgoingPackage<TPayload> package) where TPayload : notnull;
    IncomingPackage Deserialize(ReadOnlySpan<byte> data);
}
