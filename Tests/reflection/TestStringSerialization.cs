using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania.reflection;

namespace gravitymaniaTest.reflection
{
    [TestFixture]
    class TestStringSerialization
    {
        [Test]
        public void TestSerializeIntToString()
        {
            string output = StringSerialization.SerializeObject(42);

            Assert.AreEqual("42", output);

            int value = (int)StringSerialization.ParseObject(output, typeof(int));

            Assert.AreEqual(42, value);
        }

        [Test]
        public void TestSerializeUInt()
        {
            string output = StringSerialization.SerializeObject((uint)42);

            Assert.AreEqual("42", output);

            uint value = (uint)StringSerialization.ParseObject(output, typeof(uint));

            Assert.AreEqual(42, value);
        }

        [Test]
        public void TestSerializeString()
        {
            string output = StringSerialization.SerializeObject("43");

            Assert.AreEqual("43", output);

            string value = StringSerialization.ParseObject<string>(output);

            Assert.AreEqual("43", value);
        }

        [Test]
        public void TestSerializeVector2()
        {
            string output = StringSerialization.SerializeObject(new Vector2(3, 7));

            Assert.AreEqual("[3,7]", output);

            Vector2 parsed = StringSerialization.ParseObject<Vector2>(output);

            Assert.AreEqual(new Vector2(3, 7), parsed);
        }

        [Test]
        public void TestSerializeBoolean()
        {
            string output = StringSerialization.SerializeObject(false);

            Assert.AreEqual("false", output.ToLower());

            bool parsed = StringSerialization.ParseObject<bool>(output);

            Assert.AreEqual(false, parsed);

            output = StringSerialization.SerializeObject(true);

            Assert.AreEqual("true", output.ToLower());

            parsed = StringSerialization.ParseObject<bool>(output);

            Assert.AreEqual(true, parsed);
        }
    }
}
