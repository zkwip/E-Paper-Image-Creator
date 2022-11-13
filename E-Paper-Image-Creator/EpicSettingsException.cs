using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class EpicSettingsException : Exception
    {
        public EpicSettingsException()
        {
        }

        public EpicSettingsException(string? message) : base(message)
        {
        }

        public EpicSettingsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EpicSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}