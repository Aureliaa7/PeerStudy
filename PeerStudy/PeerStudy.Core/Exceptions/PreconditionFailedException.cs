using System;

namespace PeerStudy.Core.Exceptions
{
    public class PreconditionFailedException : Exception
    {
        public PreconditionFailedException() : base() { }

        public PreconditionFailedException(string message) : base(message) { }
    }
}
