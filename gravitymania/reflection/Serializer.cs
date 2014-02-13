using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.reflection
{
    interface Serializer
    {
        string Serialize(object value);
        object Parse(string str);
    }
}
