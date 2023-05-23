using Api.Features.Users;

namespace Api.Features.Messaging;

public interface IRegistrationToken : IDisposable
{
  Guid Id { get; }
  User User { get; }
}