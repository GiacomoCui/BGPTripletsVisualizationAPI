using System;

namespace Shared.DataStructures.ConcatenatedList
{
    public class ListEmptyException : Exception
    {
        public ListEmptyException() : base()
        { }

        public ListEmptyException(string message) : base(message)
        {
        }

        public ListEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}