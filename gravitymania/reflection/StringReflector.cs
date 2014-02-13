using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace gravitymania.reflection
{
	// Currently only supports get/set on properties, but eventually it would be cool to be able to support string dispatch on functions as well
    public class StringReflector
    {
        public Type TargetType { get; private set; }

        private Dictionary<string, PropertyInfo> ComponentProperties;

        public StringReflector(Type componentType)
        {
            TargetType = componentType;
            ComponentProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (PropertyInfo info in componentType.GetProperties())
            {
                if (StringSerialization.CanParse(info.PropertyType))
                {
                    ComponentProperties[info.Name] = info;
                }
            }
        }

		public IEnumerable<string> ListProperties()
		{
			return ComponentProperties.Keys;
		}

        public void SetProperty(object target, string propertyName, string value)
        {
            if (target.GetType() == TargetType)
            {
                PropertyInfo info = null;

                if (ComponentProperties.TryGetValue(propertyName, out info))
                {
                    info.GetSetMethod().Invoke(target, new object[] { StringSerialization.ParseObject(value, info.PropertyType) });
                }
                else
                {
                    throw new Exception("No property of type '" + propertyName + "' on component " + TargetType.Name);
                }
            }
            else
            {
                throw new Exception("Incorrect component type.  Expected " + TargetType.Name + ", Got " + target.GetType().Name);
            }
        }

        public string GetProperty(object target, string propertyName)
        {
            if (target.GetType() == TargetType)
            {
                PropertyInfo info = null;

                if (ComponentProperties.TryGetValue(propertyName, out info))
                {
                    return StringSerialization.SerializeObject(info.GetGetMethod().Invoke(target, null));
                }
                else
                {
                    throw new Exception("No property of type '" + propertyName + "' on component " + TargetType.Name);
                }
            }
            else
            {
                throw new Exception("Incorrect component type.  Expected " + TargetType.Name + ", Got " + target.GetType().Name);
            }
        }
    }
}
