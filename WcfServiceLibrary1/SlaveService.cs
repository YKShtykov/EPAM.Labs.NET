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
    class SlaveService: IWcfService
    {
        UserStorageAppDeployer msDeployer;
        public SlaveService()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sections = cfg.Sections;
            var settings = (InitialSettingsConfigSection)sections["initialSettings"];
            var nodes = settings.ServiceNodesItems;
            int mCount = nodes[0].Count;
            int sCount = nodes[1].Count;


            msDeployer = new UserStorageAppDeployer(0, sCount, nodes[1].FirstPort);

            msDeployer.DeployUserStorageApp();
            ConnectToMaster(nodes[0].FirstPort);
        }

        public int Add(UserContext user)
        {
            throw new Exception("Operation not allowed");
        }

        public bool Remove(UserContext user)
        {
            throw new Exception("Operation not allowed");
        }

        public bool RemoveById(int userId)
        {
            throw new Exception("Operation not allowed");
        }

        public IEnumerable<UserContext> Search(UserContext ctx)
        {
            IEnumerable<User> users = msDeployer.slaves[0].service.FindByPredicate(u => true);
            if (ctx.FirstName != null)
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
            User user = msDeployer.slaves[0].service.FindById(id);
            return ContextConverter.UserToContext(user);
        }

        private void ConnectToMaster(int firstMasterPort)
        {
            foreach (var slave in msDeployer.slaves)
            {
                slave.SetMasterPort(firstMasterPort);
            }
        }
    }
}
