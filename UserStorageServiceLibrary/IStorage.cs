using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorageServiceLibrary
{
    public interface IStorage
    {
        int Add(User user);
        bool Delete(User user);
        bool Delete(int id);
        User FindById(int id);
        IEnumerable<User> FindByPredicate(Func<User, bool> predicate);
    }
}
