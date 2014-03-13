using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using gravitymania.map;

namespace gravitymaniatest.map
{
    public class TestLoadMap
    {
        [Test]
        public void testLoadSimpleMap()
        {
            TileMap result = TileMapLoader.LoadFromText("0000111101011010", 4, 4);

            Assert.AreEqual(4, result.Width);
            Assert.AreEqual(4, result.Height);
            Assert.AreEqual(CollisionType.SolidBox, result.GetTile(0, 0).Collision);
            Assert.AreEqual(CollisionType.Empty, result.GetTile(0, 1).Collision);
            Assert.AreEqual(CollisionType.SolidBox, result.GetTile(0, 2).Collision);
            Assert.AreEqual(CollisionType.Empty, result.GetTile(0, 3).Collision);
        }
    }
}
