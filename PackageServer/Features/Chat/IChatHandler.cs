using Api.Users;

namespace Api.Features.Chat;

public interface IChatHandler<TPayload>
{
    Task Handle(IUser sender, TPayload payload, CancellationToken cancellationToken);
}
