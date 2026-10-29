using System;

namespace TestTask_CKR_System.Core.Networking
{
    public class NetworkRequestException : Exception
    {
        public long StatusCode { get; }

        public NetworkRequestException(string message, long statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}