using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class ProfileMismatchException : Exception
    {
        public ProfileMismatchException()
        {
        }

        public ProfileMismatchException(string? message) : base(message)
        {
        }

        public ProfileMismatchException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProfileMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}