using System;

namespace PeerStudy.Core.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException() : base() { }

        public DuplicateEntityException(string message) : base(message) { }
    }
}
