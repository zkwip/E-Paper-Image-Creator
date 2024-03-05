using System;
using System.Runtime.Serialization;

namespace Zkwip.EPIC
{
    [Serializable]
    internal class ProfileMismatchException(string? message) : Exception(message)
    {
    }
}