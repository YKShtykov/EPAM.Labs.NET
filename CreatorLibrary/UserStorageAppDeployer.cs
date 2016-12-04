using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServiceConnectorLibrary;
using System.Threading;
using UserStorageServiceLibrary;
using System.Configuration;

namespace CreatorLibrary
{
    public class UserStorageAppDeployer
    {
        private int mCount;
        private int sCount;
        private int currentPort;
        private int firstPort;
        public List<MasterConnector> masters;
        public List<SlaveConnector> slaves;
        private List<Thread> threads;

        public UserStorageAppDeployer(int mCount, int sCount, int firstPort)
        {
            this.mCount = mCount;
            this.sCount = sCount;
            this.firstPort = firstPort;
            currentPort = firstPort;

            masters = new List<MasterConnector>();
            slaves = new List<SlaveConnector>();
            threads = new List<Thread>();
        }
        public void DeployUserStorageApp()
        {
            for (int i = 0; i < mCount; i++)
            {
                masters.Add((MasterConnector)CreateNode(typeof(ServiceConnectorLibrary.MasterConnector)));
                currentPort++;
            }
            for (int i = 0; i < sCount; i++)
            {
                slaves.Add((SlaveConnector)CreateNode(typeof(ServiceConnectorLibrary.SlaveConnector)));
                currentPort++;
            }
            ConnectNodes();


            foreach (var master in masters)
            {
                threads.Add(new Thread(delegate ()
                {
                    master.ListenConnections(new object());
                }));
            }

            foreach (var slave in slaves)
            {
                threads.Add(new Thread(delegate ()
                {
                    slave.ListenConnection(new object());
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
            //SaveNodesState();
            //slaves[0].Find(new object());
            masters[0].SetState(new object());
            //foreach (var item in slaves)
            //{
            //    ThreadPool.QueueUserWorkItem(item.ListenConnection);
            //}  
        }

        public void CloseApp()
        {
            foreach (var master in masters)
            {
                master.Close();
            }

            foreach (var slave in slaves)
            {
                slave.Close();
            }           
        }
        //private void SaveNodesState()
        //{
        //    Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    var section = (PortConfigSection)cfg.GetSection("NodesState");

        //    if (section != null)
        //    {
        //        for (int i = 0; i < masters.Count; i++)
        //        {
        //            section.NodeItems[i].Role = "Master";
        //            section.NodeItems[i].Port = masters[i].endPoint.Port;
        //            section.NodeItems[i].XmlFile = "./" + masters[i].endPoint.Port + "Master";
        //            cfg.Save();
        //        }
        //        for (int i = masters.Count; i < masters.Count + slaves.Count; i++)
        //        {
        //            section.NodeItems[i].Role = "Slave";
        //            section.NodeItems[i].Port = slaves[i-masters.Count].endPoint.Port;
        //            section.NodeItems[i].XmlFile = "./" + slaves[i- masters.Count].endPoint.Port + "Slave";
        //            cfg.Save();
        //        }
        //    }
        //}
        private void ConnectNodes()
        {
            int[] slavePorts = slaves?.Select(s => s.endPoint.Port).ToArray();
            int[] masterPorts = masters?.Select(m => m.endPoint.Port).ToArray();
            foreach (var master in masters)
            {
                master.SetSlavePorts(slavePorts.Concat(masterPorts).ToArray());
            }
            int firstMasterPort = masters.First()?.endPoint.Port ?? -1;
            if (firstMasterPort == -1) return;
            foreach (var slave in slaves)
            {
                slave.SetMasterPort(firstMasterPort);
            }
        }

        private object CreateNode(Type type)
        {
            AppDomain slaveDomain = CreateDomain(currentPort + "Domain");
            var assemblyName = Assembly.GetAssembly(type).FullName;
            var typeName = type.FullName;
            object[] args = new object[] { currentPort };
            return slaveDomain.CreateInstanceAndUnwrap(assemblyName,
                                                                        typeName,
                                                                        false,
                                                                        BindingFlags.Default,
                                                                        null,
                                                                        args,
                                                                        null,
                                                                        null);
        }

        private AppDomain CreateDomain(string domainName)
        {
            var domainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, domainName)
            };
            return AppDomain.CreateDomain(domainName, null, domainSetup);
        }
    }
}
