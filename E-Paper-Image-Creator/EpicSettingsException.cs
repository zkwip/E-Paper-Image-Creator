using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class SettingsException : Exception
    {
        public SettingsException()
        {
        }

        public SettingsException(string? message) : base(message)
        {
        }

        public SettingsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}