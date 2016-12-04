using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreatorLibrary;
using System.Configuration;

namespace CreatorLibrary
{
    /// <summary>
    /// Class for custom config section creating
    /// </summary>
    public class PortConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("nodes")]
        public NodesCollection NodeItems
        {
            get { return ((NodesCollection)(base["nodes"])); }
        }
        public override bool IsReadOnly()
        {
            return false;
        }
    }

    [ConfigurationCollection(typeof(NodeElement), AddItemName = "Node")]
    public class NodesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new NodeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NodeElement)(element)).Port;
        }

        public NodeElement this[int idx]
        {
            get { return (NodeElement)BaseGet(idx); }
        }
    }

    public class NodeElement : ConfigurationElement
    {

        [ConfigurationProperty("file", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string XmlFile
        {
            get { return ((string)(base["file"])); }
            set { base["file"] = value; }
        }
        [ConfigurationProperty("role", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Role
        {
            get { return ((string)(base["role"])); }
            set { base["role"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = "0", IsKey = false, IsRequired = false)]
        public int Port
        {
            get { return ((int)(base["port"])); }
            set { base["port"] = value; }
        }
    }
}
