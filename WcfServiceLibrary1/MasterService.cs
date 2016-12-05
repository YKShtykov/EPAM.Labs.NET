using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CreatorLibrary;
using UserStorageServiceLibrary;

namespace WcfServiceLibrary1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class MasterService : IWcfService
    {
        public static UserStorageAppDeployer msDeployer;
        static MasterService()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sections = cfg.Sections;
            var settings = (InitialSettingsConfigSection)sections["initialSettings"];
            var nodes = settings.ServiceNodesItems;
            int mCount = nodes[0].Count;
            int sCount = nodes[1].Count;


            msDeployer = new UserStorageAppDeployer(mCount, 0, nodes[0].FirstPort);
            
            msDeployer.DeployUserStorageApp();
            ConnectToSlaves(sCount,mCount, nodes[1].FirstPort,nodes[0].FirstPort);
        }

        public int Add(UserContext user)
        {
            User u = ContextConverter.ContextToUser(user);
            msDeployer.masters[0].service.Add(u);
            return u.personalId;
        }

        public bool Remove(UserContext user)
        {
            User u = ContextConverter.ContextToUser(user);
            return msDeployer.masters[0].service.Remove(u);
        }

        public bool RemoveById(int userId)
        {
            return msDeployer.masters[0].service.Remove(userId);
        }

        public IEnumerable<UserContext> Search(UserContext ctx)
        {
            IEnumerable<User> users = msDeployer.masters[0].service.FindByPredicate(u=> true);
            if (ctx.FirstName!= null)
            {
                users = users.Where(u => u.FirstName == ctx.FirstName);
            }
            if (ctx.LastName != null)
            {
                users = users.Where(u => u.LastName == ctx.LastName);
            }
            if (ctx.DateOfBirth != default(DateTime))
            {
                users = users.Where(u => u.BirthDate == ctx.DateOfBirth);
            }
            users = users.Where(u => u.Gender == ctx.Gender);

            return users.Select(u => ContextConverter.UserToContext(u));
        }

        public UserContext SearchById(int id)
        {
            User user= msDeployer.masters[0].service.FindById(id);
            return ContextConverter.UserToContext(user);
        }

        private static void ConnectToSlaves(int sCount,int mCount ,int firstSlavePort, int firstMasterPort)
        {
            int[] slavesPorts = new int[sCount];
            int[] masterPorts = new int[mCount];
            for (int i = 0; i < sCount; i++)
            {
                slavesPorts[i] = firstSlavePort + i;
            }

            for (int i = 0; i < mCount; i++)
            {
                masterPorts[i] = firstMasterPort + i;
            }

            foreach (var master in msDeployer.masters)
            {
                master.SetSlavePorts(slavesPorts.Concat(masterPorts).ToArray());
            }
        }
    }
}
