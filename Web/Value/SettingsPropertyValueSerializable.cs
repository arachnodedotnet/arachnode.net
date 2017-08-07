using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Arachnode.Web.Value
{
    [Serializable]
    public class SettingsPropertyValueSerializable : SettingsPropertyValue
    {
        public SettingsPropertyValueSerializable() : base(new SettingsProperty(string.Empty))
        {
        }

        public SettingsPropertyValueSerializable(SettingsProperty property) : base(property)
        {
        }
    }
}
