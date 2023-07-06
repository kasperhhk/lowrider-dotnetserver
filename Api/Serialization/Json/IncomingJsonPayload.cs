using Api.Messaging;
using System.Text.Json;

namespace Api.Serialization.Json;

public class IncomingJsonPayload : IIncomingPayload
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly string _payload;

    public IncomingJsonPayload(JsonSerializerOptions jsonSerializerOptions, string payload)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        _payload = payload;
    }

    public T Deserialize<T>()
    {
        var result = JsonSerializer.Deserialize<T>(_payload, _jsonSerializerOptions);
        if (result is null)
            throw new MessageFormatException("Invalid payload format");

        return result;
    }
}
