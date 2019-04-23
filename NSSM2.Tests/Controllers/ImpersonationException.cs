using System;
using System.Runtime.Serialization;

namespace NSSM2.Tests.Controllers
{
    [Serializable]
    internal class ImpersonationException : Exception
    {
        private int win32ErrorNumber;
        private string message;
        private string username;
        private string domain;

        public ImpersonationException()
        {
        }

        public ImpersonationException(string message) : base(message)
        {
        }

        public ImpersonationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ImpersonationException(int win32ErrorNumber, string message, string username, string domain)
        {
            this.win32ErrorNumber = win32ErrorNumber;
            this.message = message;
            this.username = username;
            this.domain = domain;
        }

        protected ImpersonationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}