using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreatorLibrary;
using System.Configuration;
namespace WcfServiceLibrary1
{
    public class InitialSettingsConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("serviceNodes")]
        public NodesCollection ServiceNodesItems
        {
            get { return ((NodesCollection)(base["serviceNodes"])); }
        }
    }

    [ConfigurationCollection(typeof(ServiceNode))]
    public class NodesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceNode();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceNode)(element)).NodeType;
        }

        public ServiceNode this[int idx]
        {
            get { return (ServiceNode)BaseGet(idx); }
        }
    }

    public class ServiceNode : ConfigurationElement
    {

        [ConfigurationProperty("nodeType", DefaultValue = "Slave", IsKey = true, IsRequired = true)]
        public string NodeType
        {
            get { return ((string)(base["nodeType"])); }
            set { base["nodeType"] = value; }
        }

        [ConfigurationProperty("count", DefaultValue = 0, IsKey = false, IsRequired = true)]
        public int Count
        {
            get { return ((int)(base["count"])); }
            set { base["count"] = value; }
        }

        [ConfigurationProperty("firstPort", DefaultValue = 0, IsKey = false, IsRequired = true)]
        public int FirstPort
        {
            get { return ((int)(base["firstPort"])); }
            set { base["firstPort"] = value; }
        }
    }
}
