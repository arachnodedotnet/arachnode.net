using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Arachnode.Web.Value
{
    //[Serializable]
    public class SettingsPropertySerializable : SettingsProperty
    {
        public SettingsPropertySerializable() : base(string.Empty)
        {
        }

        public SettingsPropertySerializable(string name) : base(name)
        {
        }

        public SettingsPropertySerializable(string name, Type propertyType, SettingsProvider provider, bool isReadOnly, object defaultValue, SettingsSerializeAs serializeAs, SettingsAttributeDictionary attributes, bool throwOnErrorDeserializing, bool throwOnErrorSerializing) : base(name, propertyType, provider, isReadOnly, defaultValue, serializeAs, attributes, throwOnErrorDeserializing, throwOnErrorSerializing)
        {
        }

        public SettingsPropertySerializable(SettingsProperty propertyToCopy) : base(propertyToCopy)
        {
        }
    }
}
