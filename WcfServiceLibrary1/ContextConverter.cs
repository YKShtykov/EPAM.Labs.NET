using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserStorageServiceLibrary;

namespace WcfServiceLibrary1
{
    public class ContextConverter
    {
        public static UserContext UserToContext(User user)
        {
            if (ReferenceEquals(user, null)) return null;
            return new UserContext()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.BirthDate,
                Gender = user.Gender,
                PersonalId= user.personalId
            };
        }
        public static User ContextToUser(UserContext user)
        {
            if (ReferenceEquals(user, null)) return null;
            return new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.DateOfBirth,
                Gender = user.Gender,
                personalId=user.PersonalId
            };
        }
    }
}
