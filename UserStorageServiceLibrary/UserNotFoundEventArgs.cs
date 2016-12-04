using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorageServiceLibrary
{
    [Serializable]
    public class UserNotFoundEventArgs: EventArgs
    {
        public readonly Func<User, bool> predicate;
        public readonly int personalId;

        public UserNotFoundEventArgs(Func<User, bool> predicate, int personalId)
        {
            this.predicate = predicate;
            this.personalId = personalId;
        }
    }
}
