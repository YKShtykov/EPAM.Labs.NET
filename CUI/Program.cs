using System;
using CreatorLibrary;
using System.Configuration;

namespace CUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sections = cfg.Sections;
            var settings = (InitialSettingsConfigSection)sections["initialSettings"];
            var nodes = settings.ServiceNodesItems;
            int mCount = nodes[0].Count;
            int sCount = nodes[1].Count;


            var msDeployer = new UserStorageAppDeployer(mCount,sCount, 10000);

            msDeployer.DeployUserStorageApp();
            msDeployer.ConnectNodes();
            msDeployer.CloseApp();
            //Console.ReadKey();
        }
    }

   
}
