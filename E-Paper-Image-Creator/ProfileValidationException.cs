using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class ProfileValidationException : Exception
    {
        public ProfileValidationException(string? message) : base(message)
        {
        }

        public ProfileValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}