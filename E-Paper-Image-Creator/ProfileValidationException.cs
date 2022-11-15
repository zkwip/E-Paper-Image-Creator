using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class ProfileValidationException : Exception
    {
        public ProfileValidationException()
        {
        }

        public ProfileValidationException(string? message) : base(message)
        {
        }

        public ProfileValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProfileValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}