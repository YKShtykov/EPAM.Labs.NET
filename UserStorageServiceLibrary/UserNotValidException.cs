using System;
using System.Runtime.Serialization;

namespace UserStorageServiceLibrary
{
    /// <summary>
    /// Exception throws when user is not valid
    /// </summary>
    [Serializable]
    internal class UserNotValidException : Exception
    {
        public UserNotValidException()
        {
        }

        public UserNotValidException(string message) : base(message)
        {
        }

        public UserNotValidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}