namespace Api.Features.Messaging;

public interface IMessage {};
public record OutgoingMessage(string Type, string JsonData) : IMessage;
public record IncomingMessage(string Type, string JsonData) : IMessage;


public record IncomingChatMessage(string message) : IMessage;
public record OutgoingChatMessage(string sender, string message) : IMessage;
