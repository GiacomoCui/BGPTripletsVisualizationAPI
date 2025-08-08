namespace Shared.DataStructures.ConcatenatedList
{
    public class ConcatenatedLinkedListNode<T>
    {
        public ConcatenatedLinkedListNode<T> Next { get; internal set; }
        public ConcatenatedLinkedListNode<T> Prev { get; internal set; }

        public T Value { get; set; }

        public ConcatenatedLinkedListNode(T value)
        {
            Value = value;
        }
    }
}