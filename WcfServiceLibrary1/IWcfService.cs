using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using UserStorageServiceLibrary;
using System.Text;

namespace WcfServiceLibrary1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IWcfService
    {
        [OperationContract]
        UserContext SearchById(int id);

        [OperationContract]
        IEnumerable<UserContext> Search(UserContext ctx);

        [OperationContract]
        int Add(UserContext user);

        [OperationContract]
        bool RemoveById(int userId);
        [OperationContract]
        bool Remove(UserContext user);
    }

    [DataContract]
    public class UserContext
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public int PersonalId { get; set; }
    }
}

