using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania;
using gravitymania.console;
using Microsoft.Xna.Framework.Input;
using gravitymania.input;

namespace gravitymaniaTest.input
{
    [TestFixture]
    class TestKeyMap
    {
        [Test]
        public void TestGetAllModifiers()
        {
            KeyModifiers mods1 = KeyModifiersMethods.GetModifiers(Keys.A, Keys.LeftAlt, Keys.B);

            Assert.AreEqual(KeyModifiers.Alt, mods1);

            KeyModifiers mods2 = KeyModifiersMethods.GetModifiers(Keys.D3, Keys.W, Keys.RightShift);

            Assert.AreEqual(KeyModifiers.Shift, mods2);

            KeyModifiers mods3 = KeyModifiersMethods.GetModifiers(Keys.LeftControl, Keys.W, Keys.X);

            Assert.AreEqual(KeyModifiers.Ctrl, mods3);

            KeyModifiers mods4 = KeyModifiersMethods.GetModifiers(Keys.T, Keys.RightControl, Keys.LeftShift, Keys.X);

            Assert.AreEqual(KeyModifiers.Ctrl | KeyModifiers.Shift, mods4);

            KeyModifiers mods5 = KeyModifiersMethods.GetModifiers(Keys.T, Keys.RightControl, Keys.LeftShift, Keys.RightAlt, Keys.X);

            Assert.AreEqual(KeyModifiers.Ctrl | KeyModifiers.Shift | KeyModifiers.Alt, mods5);
        }

        [Test]
        public void TestKeyLookup()
        {
            KeyMap map = KeyMap.USKeyboard;

            char c = map.GetCharacter(Keys.A);

            Assert.AreEqual('a', c);

            char slash = map.GetCharacter(Keys.OemQuestion);

            Assert.AreEqual('/', slash);

            char colon = map.GetCharacter(KeyModifiers.Shift, Keys.OemSemicolon);

            Assert.AreEqual(':', colon);
        }
        
        [Test]
        public void TestReverseKeyLookup()
        {
            KeyMap map = KeyMap.USKeyboard;

            KeymapKey k = map.GetKeymapKey('a');

            Assert.AreEqual(Keys.A, k.Key);
            Assert.AreEqual(KeyModifiers.None, k.Modifiers);
        }
    }
}
