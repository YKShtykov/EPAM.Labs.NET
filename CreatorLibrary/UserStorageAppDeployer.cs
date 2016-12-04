using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ServiceConnectorLibrary;
using System.Threading;

namespace CreatorLibrary
{

    /// <summary>
    /// Class-deployer for Master-Slave application.
    /// It sreates several master and slave nodes and connects them.
    /// </summary>
    public class UserStorageAppDeployer
    {
        /// <summary>
        /// masters count
        /// </summary>
        private int mCount;
        /// <summary>
        /// Slaves count
        /// </summary>
        private int sCount;
        /// <summary>
        /// portNumber for connecting Node to it
        /// </summary>
        private int currentPort;
        /// <summary>
        /// first port the iteration begins from
        /// </summary>
        private int firstPort;
        /// <summary>
        /// List of master nodes
        /// </summary>
        public List<MasterNode> masters;
        /// <summary>
        /// List of slaves
        /// </summary>
        public List<SlaveNode> slaves;
        /// <summary>
        /// List of theads. Each node listens its port in individual thread
        /// </summary>
        private List<Thread> threads;

        /// <summary>
        /// Sets parameters for future work
        /// </summary>
        /// <param name="mCount">count of masters</param>
        /// <param name="sCount">count of slaves</param>
        /// <param name="firstPort"></param>
        public UserStorageAppDeployer(int mCount, int sCount, int firstPort)
        {
            this.mCount = mCount;
            this.sCount = sCount;
            this.firstPort = firstPort;
            currentPort = firstPort;

            masters = new List<MasterNode>();
            slaves = new List<SlaveNode>();
            threads = new List<Thread>();
        }
        /// <summary>
        /// Deploys master and slaves nodes and run its;
        /// </summary>
        public void DeployUserStorageApp()
        {
            DeployMasters();
            DeploySlaves();


            foreach (var thread in threads)
            {
                thread.Start();
            }
        }

        /// <summary>
        /// Stops master slave application and saves state of services
        /// </summary>
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

        /// <summary>
        /// Deploys master nodes
        /// </summary>
        private void DeployMasters()
        {
            for (int i = 0; i < mCount; i++)
            {
                masters.Add((MasterNode)CreateNode(typeof(ServiceConnectorLibrary.MasterNode)));
                currentPort++;
            }

            foreach (var master in masters)
            {
                threads.Add(new Thread(delegate ()
                {
                    master.ListenConnections(new object());
                }));
            }
        }

        /// <summary>
        /// Deploys slaves nodes
        /// </summary>
        private void DeploySlaves()
        {
            for (int i = 0; i < sCount; i++)
            {
                slaves.Add((SlaveNode)CreateNode(typeof(ServiceConnectorLibrary.SlaveNode)));
                currentPort++;
            }

            foreach (var slave in slaves)
            {
                threads.Add(new Thread(delegate ()
                {
                    slave.ListenConnection(new object());
                }));
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

        
        /// <summary>
        /// If slaves and masters are created in one instance of deployer, they can be connected by this method;
        /// </summary>
        public void ConnectNodes()
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

        /// <summary>
        /// Create node of master or slave type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create separate domen for each node
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
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
