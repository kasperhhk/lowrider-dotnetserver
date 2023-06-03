namespace Api.Features.Serialization.Json;

public interface IJsonValueFactory
{
  T JsonDeserialize<T>(ReadOnlySpan<char> payload);
}