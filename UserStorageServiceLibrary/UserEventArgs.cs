using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorageServiceLibrary
{
    [Serializable]
    public class UserEventArgs: EventArgs
    {
        public readonly string operation;
        public readonly User user;

        public UserEventArgs(string operation, User user)
        {
            this.operation = operation;
            this.user = user;
        }

    }
}
