using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorageServiceLibrary;

namespace ServiceConnectorLibrary
{
    public interface IMessage
    {
    }

    [Serializable]
    public class FindMessage:IMessage
    {
        public int senderPort;
        public int id;
        public Func<User, bool> predicate;
    }

    [Serializable]
    public class UpdateMessage:IMessage
    {
        public int senderPort;
        public string operation;
        public User item;
    }
}
