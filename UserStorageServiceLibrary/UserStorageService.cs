using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UserStorageServiceLibrary
{
    public class UserStorageService: IDisposable
    {
        readonly IStorage storage;
        public Func<User, bool> AdditionalUserValidation;
        public event EventHandler<UserEventArgs> StateChanged = delegate { };
        public event EventHandler<UserNotFoundEventArgs> UserNotFound = delegate { };

        public UserStorageService()
        {
            storage = new DefaultStorage();
        }
        public UserStorageService(IStorage storage)
        {
            this.storage = storage;
        }
        public int Add(User user)
        {
            if (ReferenceEquals(user, null)) throw new ArgumentNullException();

            if (!ReferenceEquals(AdditionalUserValidation, null))
                if (!AdditionalUserValidation(user)) throw new UserNotValidException();

            return storage.Add(user);

        }

        public bool Remove(User user)
        {
            if (ReferenceEquals(user, null)) throw new ArgumentNullException();

            return storage.Delete(user);
        }

        public bool Remove(int userId)
        {
            return storage.Delete(userId);
        }

        public User FindById(int userId)
        {
            return storage.FindById(userId);
        }

        public IEnumerable<User> FindByPredicate(Func<User,bool> predicate)
        {
            if (ReferenceEquals(predicate, null)) throw new ArgumentNullException();

            return storage.FindByPredicate(predicate);
        }

        private void OnStateChanged(string operation, User user)
        {
            var evArgs = new UserEventArgs(operation, user);
            StateChanged(this, evArgs);
        }

        private void OnUserNotFound(Func<User,bool> predicate, int personalId)
        {
            var evArgs = new UserNotFoundEventArgs(predicate, personalId);
            UserNotFound(this, evArgs);
        }

        public void Dispose()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(User));
            using (FileStream fs = new FileStream("users.xml", FileMode.OpenOrCreate))
            {
                List<User> allusers = storage.FindByPredicate(u => true).ToList();
                foreach (var user in allusers)
                {
                    formatter.Serialize(fs, user);
                }

                Console.WriteLine("user storage service state was serialized");
            }
        }
    }    
}
