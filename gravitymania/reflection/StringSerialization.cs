using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace gravitymania.reflection
{
    public class Vector2Serializer : Serializer
    {
        public string Serialize(object value)
        {
            Vector2 vec = (Vector2) value;
            return "[" + vec.X + "," + vec.Y + "]";
        }

        public object Parse(string str)
        {
            int start = str.IndexOf('[');
            int end = str.LastIndexOf(']');

            string[] toks = str.Substring(start + 1, end - start - 1).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (toks.Length == 2)
            {
                return new Vector2(float.Parse(toks[0]), float.Parse(toks[1]));
            }
            else
            {
                throw new Exception("Invalid vector format: " + str);
            }
        }
    }

    public static class StringSerialization
    {
        private static Dictionary<Type, Serializer> CustomSerializers = new Dictionary<Type,Serializer>()
        {
            { typeof(Vector2), new Vector2Serializer() },
        };

        public static T ParseObject<T>(string value)
        {
            return (T)ParseObject(value, typeof(T));
        }

        public static object ParseObject(string value, Type type)
        {
            if (CustomSerializers.ContainsKey(type))
            {
                return CustomSerializers[type].Parse(value);
            }
            else if (type == typeof(string))
            {
                return value;
            }
            else if (type.IsEnum)
            {
                MethodInfo method = typeof(StringSerialization).GetMethod("GetSerializedEnum");
                return method.MakeGenericMethod(type).Invoke(null, new object[] { value, StringComparison.OrdinalIgnoreCase });
            }
            else
            {
                MethodInfo method = type.GetMethod("Parse", new Type[] { typeof(string) });

                if (method != null)
                {
                    return method.Invoke(null, new object[] { value }); 
                }
                else
                {
                    throw new Exception("No parse availiable for type " + type.ToString());
                }
            }
        }

        public static bool CanParse(this Type objectType)
        {
            if (CustomSerializers.ContainsKey(objectType))
            {
                return true;
            }
            else if (objectType.IsEnum)
            {
                return true;
            }
            else
            {
                MethodInfo method = objectType.GetMethod("Parse", new Type[] { typeof(string) });

                if (method != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static string SerializeObject(this object value)
        {
            Type objectType = value.GetType();

            string result = null;

            if (CustomSerializers.ContainsKey(objectType))
            {
                result = CustomSerializers[objectType].Serialize(value);
            }
            else if (objectType.IsEnum)
            {
                result = GetEnumSerializationValue((Enum)value);
            }


            if (result == null)
            {
                result = value.ToString();
            }

            return result;
        }

        public static string GetEnumSerializationValue(this Enum value)
        {
            // Ugly stuff!
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);

            StringSerializationAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(StringSerializationAttribute), false) as StringSerializationAttribute[];

            return attributes.Any() ? attributes[0].Value : null;
        }

        public static bool TryGetSerializedEnum<T>(string value, out T result, StringComparison comparison = StringComparison.Ordinal)
        {
            // Even uglier stuff!
            Array a = Enum.GetValues(typeof(T));

            result = default(T);

            foreach (Enum e in a)
            {
                if (string.Compare(e.GetEnumSerializationValue(), value, comparison) == 0)
                {
                    // wow, I'm not even sure if this makes sense...
                    result = (T)(object)e;
                    return true;
                }
            }

            return false;
        }

        public static T GetSerializedEnum<T>(string value, StringComparison comparison = StringComparison.Ordinal)
        {
            T result;

            if (!TryGetSerializedEnum<T>(value, out result, comparison))
            {
                throw new Exception("No such enum '" + value + "'");
            }

            return result;
        }
    }
}
