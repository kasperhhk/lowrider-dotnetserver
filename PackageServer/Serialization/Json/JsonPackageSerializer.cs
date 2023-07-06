using Api.Messaging.Models;
using System.Text;
using System.Text.Json;

namespace Api.Serialization.Json;

public interface IJsonPackageSerializer
{
    IncomingPackage Deserialize(ReadOnlySpan<byte> data);
    byte[] Serialize<TPayload>(OutgoingPackage<TPayload> package) where TPayload : notnull;
}

public class JsonPackageSerializer : IJsonPackageSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public JsonPackageSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public IncomingPackage Deserialize(ReadOnlySpan<byte> data)
    {
        var str = Encoding.UTF8.GetString(data);
        var first = str.IndexOf(':');
        if (first is -1)
            throw new MessageFormatException("Invalid message identifiers");

        var second = str.IndexOf(':', first + 1);
        if (second is -1)
            throw new MessageFormatException("Invalid message identifiers");

        if (second == str.Length - 1)
            throw new MessageFormatException("Missing payload");

        var feature = str.Substring(0, first);
        var command = str.Substring(first + 1, second);
        var jsonPayload = str.Substring(second + 1);

        return new IncomingPackage(feature, command, new IncomingJsonPayload(_jsonSerializerOptions, jsonPayload));
    }

    public byte[] Serialize<TPayload>(OutgoingPackage<TPayload> package) where TPayload : notnull
    {
        var jsonPayload = JsonSerializer.Serialize(package.Payload, _jsonSerializerOptions);
        var str = $"{package.Feature}:{package.Command}:{jsonPayload}";
        var bytes = Encoding.UTF8.GetBytes(str);

        return bytes;
    }
}
