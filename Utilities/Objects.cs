using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Arachnode.Utilities
{
    public static class Objects
    {
        public static string GetPropertyAndPropertyValuesString(object o)
        {
            if (o == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach(PropertyInfo propertyInfo in o.GetType().GetProperties())
            {
                stringBuilder.Append(propertyInfo.Name + ":" + propertyInfo.GetValue(o, null) + "|");
            }

            return stringBuilder.ToString().TrimEnd("|".ToCharArray());
        }
    }
}
