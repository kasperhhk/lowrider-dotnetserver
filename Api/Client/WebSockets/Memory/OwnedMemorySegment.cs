using System.Buffers;

namespace Api.Client.WebSockets.Memory;

public class OwnedMemorySegment<T> : ReadOnlySequenceSegment<T>
{
    public IMemoryOwner<T> Owner { get; }
    public OwnedMemorySegment<T>? NextOwned => (OwnedMemorySegment<T>?)Next;

    public OwnedMemorySegment(IMemoryOwner<T> owner, ReadOnlyMemory<T> ownedSlice) => (Owner, Memory) = (owner, ownedSlice);

    public OwnedMemorySegment<T> Add(IMemoryOwner<T> owner, ReadOnlyMemory<T> ownedSlice)
    {
        var segment = new OwnedMemorySegment<T>(owner, ownedSlice);
        segment.RunningIndex = RunningIndex + Memory.Length;

        Next = segment;
        return segment;
    }
}