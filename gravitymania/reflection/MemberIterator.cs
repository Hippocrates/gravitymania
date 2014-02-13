using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace gravitymania.reflection
{
	public static class MemberIterator
	{
		public static IEnumerable<FieldInfo> ListFields(Type t)
		{
			foreach (FieldInfo f in t.GetFields())
			{
				yield return f;
			}
		}

		public static IEnumerable<Type> ListFieldTypes(Type t)
		{
			return ListFields(t).Select(f => f.FieldType);
		}
	}
}
