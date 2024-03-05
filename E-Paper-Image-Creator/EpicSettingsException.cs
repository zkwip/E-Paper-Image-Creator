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
    }
}