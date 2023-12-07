using System.Configuration;
using System.Xml;

namespace Aggregation.Deployment
{
    public class ConfigElement : ConfigurationElement
    {
        public string InnerText { get; private set; }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            InnerText = reader.ReadElementContentAsString();
        }
    }
}
