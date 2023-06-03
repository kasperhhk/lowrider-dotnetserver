using System.Buffers;

namespace K;

public class WebSocketMessageFrame : IDisposable
{
  private bool _disposed = false;

  private OwnedMemorySegment<byte>? _first = null;
  private OwnedMemorySegment<byte>? _last = null;

  public long Size => _last == null ? 0 : _last.RunningIndex + _last.Memory.Length;

  // Use `out` because we're dealing with a struct
  public void GetMessageSequence(out ReadOnlySequence<byte> messageSequence)
  {
    if (_first == null || _last == null)
      throw new InvalidOperationException();

    messageSequence = new ReadOnlySequence<byte>(_first, 0, _last, _last.Memory.Length);
  }

  public void Add(IMemoryOwner<byte> owner, ReadOnlyMemory<byte> ownedMemory)
  {
    if (_first == null)
      _first = _last = new OwnedMemorySegment<byte>(owner, ownedMemory);
    else
      _last = _last!.Add(owner, ownedMemory);
  }

  public void Reset()
  {
    Dispose();

    _first = null;
    _last = null;
    _disposed = false;
  }

  public void Dispose()
  {
    if (_disposed)
      return;

    var segment = _first;
    while (segment != null)
    {
      segment.Owner.Dispose();

      segment = segment.NextOwned;
    }

    _disposed = true;
  }
}