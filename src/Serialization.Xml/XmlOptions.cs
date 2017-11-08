using System;
using MsXmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace Serialization.Xml
{
    /// <summary>
    /// Options for the Xml Serialization
    /// </summary>
    public class XmlOptions
    {
        /// <summary>
        /// Default xml namespace
        /// </summary>
        public string DefaultNamespace { get; set; }
        /// <summary>
        /// Factory to instantiate an XmlSerializer
        /// </summary>
        public Func<Type, MsXmlSerializer> Factory { get; set; }
    }
}