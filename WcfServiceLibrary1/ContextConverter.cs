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
