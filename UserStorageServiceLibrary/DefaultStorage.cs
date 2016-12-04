using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorageServiceLibrary
{
    [Serializable]
    public class DefaultStorage : IStorage
    {
        private List<User> store;

        public DefaultStorage()
        {
            store = new List<User>();
        }
        public int Add(User user)
        {
            if (store.Contains(user)) throw new Exception();
            store.Add(user);

            return user.personalId;
        }

        public bool Delete(int id) => Delete(u => u.personalId == id);

        public bool Delete(User user) => Delete(u => u == user);

        private bool Delete(Func<User, bool> predicate)
        {
            var user = store.FirstOrDefault(predicate);
            if (ReferenceEquals(user, null)) return false;

            store.Remove(user);

            return true;
        }

        public User FindById(int id)
        {
            var user = store.FirstOrDefault(u => u.personalId == id);

            return ReferenceEquals(user, null)? null : (User)user.Clone();
        }

        public IEnumerable<User> FindByPredicate(Func<User, bool> predicate)
        {
            return store.Where(predicate).Select(u => (User)u.Clone());
        }
    }
}
