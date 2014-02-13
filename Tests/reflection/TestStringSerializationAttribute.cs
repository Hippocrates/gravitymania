using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using gravitymania.reflection;

namespace gravitymaniaTest.reflection
{
    [TestFixture]
    class TestStringSerializationAttribute
    {
        enum DummyEnum
        {
            [StringSerialization("xxy")]
            One,

            [StringSerialization("xyy")]
            Two,

            Three,

            [StringSerialization("yyy")]
            Four,
        }


        [Test]
        public void EnumToString()
        {
            Assert.AreEqual("xxy", DummyEnum.One.GetEnumSerializationValue());
        }

        [Test]
        public void EnumToStringNoString()
        {
            Assert.IsNull(DummyEnum.Three.GetEnumSerializationValue());
        }

        [Test]
        public void StringToEnum()
        {
            Assert.AreEqual(DummyEnum.Four, StringSerialization.GetSerializedEnum<DummyEnum>("yyy"));
        }

        [Test]
        public void StringToEnumIgnoreCase()
        {
            Assert.AreEqual(DummyEnum.Two, StringSerialization.GetSerializedEnum<DummyEnum>("xYy", StringComparison.OrdinalIgnoreCase));
        }
    }
}
