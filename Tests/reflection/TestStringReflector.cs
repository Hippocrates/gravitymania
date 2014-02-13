using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania.reflection;

namespace gravitymaniaTest.reflection
{
    class DummyComponent
    {
        public int X { get; set; }
        public float Y { get; set; }
        public Vector2 Z { get; set; }
    }

    [TestFixture]
    class TestStringReflector
    {
        [Test]
        public void TestReadComponentProperties()
        {
            DummyComponent c = new DummyComponent() { X = 3, Y = 6.7f, Z = new Vector2(55.4f, 66.5f) };

			StringReflector access = new StringReflector(typeof(DummyComponent));

            Assert.AreEqual(c.X.ToString(), access.GetProperty(c, "X"));
            Assert.AreEqual(c.Y.ToString(), access.GetProperty(c, "Y"));
            Assert.AreEqual(StringSerialization.SerializeObject(c.Z), access.GetProperty(c, "Z"));
        }

        [Test]
        public void TestWriteComponentProperties()
        {
            DummyComponent c = new DummyComponent() { X = 3, Y = 6.7f, Z = new Vector2(55.4f, 66.5f) };

			StringReflector access = new StringReflector(typeof(DummyComponent));

            int newX = 33;
            float newY = 44.5f;
            Vector2 newZ = new Vector2(22.3f, 33.4f);

            access.SetProperty(c, "X", newX.ToString());
            access.SetProperty(c, "Y", newY.ToString());
            access.SetProperty(c, "Z", StringSerialization.SerializeObject(newZ));

            Assert.AreEqual(newX, c.X);
            Assert.AreEqual(newY, c.Y);
            Assert.AreEqual(newZ.X, c.Z.X);
            Assert.AreEqual(newZ.Y, c.Z.Y);

            Assert.AreEqual(newX.ToString(), access.GetProperty(c, "X"));
            Assert.AreEqual(newY.ToString(), access.GetProperty(c, "Y"));
            Assert.AreEqual(StringSerialization.SerializeObject(newZ), access.GetProperty(c, "Z"));
        }
    }
}
