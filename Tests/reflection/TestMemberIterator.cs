using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania.reflection;

namespace gravitymaniaTest.reflection
{
	public class SimpleObject
	{
		public int x;
		public float y;
		public string z;
	}

	[TestFixture]  
	public class TestMemberIterator
	{
		[Test]
		public void TestFieldIterator()
		{
			bool foundX = false;
			bool foundY = false;
			bool foundZ = false;

			foreach (var f in MemberIterator.ListFields(typeof(SimpleObject)))
			{
				if (f.Name == "x")
				{
					foundX = true;
					Assert.IsTrue(f.FieldType.Equals(typeof(int)));
				}
				else if (f.Name == "y")
				{
					foundY = true;
					Assert.IsTrue(f.FieldType.Equals(typeof(float)));
				}
				else if (f.Name == "z")
				{
					foundZ = true;
					Assert.IsTrue(f.FieldType.Equals(typeof(string)));
				}
			}

			Assert.IsTrue(foundX);
			Assert.IsTrue(foundY);
			Assert.IsTrue(foundZ);
		}

	}
}
