using System.Runtime.Serialization;

namespace Api.Messaging;

public class MessageFormatException : MessageException
{
    public MessageFormatException()
    {
    }

    public MessageFormatException(string? message) : base(message)
    {
    }

    public MessageFormatException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected MessageFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
