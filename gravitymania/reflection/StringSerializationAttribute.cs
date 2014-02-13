using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.reflection
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StringSerializationAttribute : Attribute
    {
        public readonly string Value;

        public StringSerializationAttribute(string value)
        {
            Value = value;
        }
    }
}
