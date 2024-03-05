using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class FileReaderException(string? message) : Exception(message)
    {
    }
}