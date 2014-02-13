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

namespace gravitymaniaTest.console
{
    [TestFixture]
    class TestConsoleKeyRepeat
    {
        [Test]
        public void TestEmptyInitialState()
        {
            ConsoleKeyReader repeat = new ConsoleKeyReader();

            for (int i = 0; i < 10; ++i)
            {
                KeyboardState state = new KeyboardState();
                repeat.Update(state);

                Assert.AreEqual(Keys.None, repeat.GetCurrentKey());
            }
        }

        [Test]
        public void TestInitialPress()
        {
            ConsoleKeyReader repeat = new ConsoleKeyReader();

            KeyboardState first = new KeyboardState(Keys.A);

            repeat.Update(first);

            Assert.AreEqual(Keys.A, repeat.GetCurrentKey());

            KeyboardState second = new KeyboardState(Keys.A);

            repeat.Update(first);

            Assert.AreEqual(Keys.None, repeat.GetCurrentKey());

            KeyboardState third = new KeyboardState();

            repeat.Update(third);

            Assert.AreEqual(Keys.None, repeat.GetCurrentKey());

            KeyboardState fourth = new KeyboardState(Keys.A);

            repeat.Update(fourth);

            Assert.AreEqual(Keys.A, repeat.GetCurrentKey());
        }

        [Test]
        public void TestInitialKeyRapidPressRelease()
        {
            ConsoleKeyReader repeat = new ConsoleKeyReader();

            KeyboardState on = new KeyboardState(Keys.A);
            KeyboardState off = new KeyboardState();

            for (int i = 0; i < 1000; ++i)
            {
                repeat.Update(on);
                Assert.AreEqual(Keys.A, repeat.GetCurrentKey());
                repeat.Update(off);
                Assert.AreEqual(Keys.None, repeat.GetCurrentKey());
            }
        }

        [Test]
        public void TestKeyRepeat()
        {
            ConsoleKeyReader repeat = new ConsoleKeyReader();

            KeyboardState on = new KeyboardState(Keys.A);

            for (int i = 0; i < repeat.InitialKeyRepeat + repeat.HeldKeyRepeat; ++i)
            {
                repeat.Update(on);
                if (i % repeat.InitialKeyRepeat == 0)
                {
                    Assert.AreEqual(Keys.A, repeat.GetCurrentKey());
                }
                else
                {
                    Assert.AreEqual(Keys.None, repeat.GetCurrentKey());
                }
            }

            for (int i = 0; i < 1000; ++i)
            {
                repeat.Update(on);
                if (i % repeat.HeldKeyRepeat == 0)
                {
                    Assert.AreEqual(Keys.A, repeat.GetCurrentKey());
                }
                else
                {
                    Assert.AreEqual(Keys.None, repeat.GetCurrentKey());
                }
            }
        }

        [Test]
        public void TestNewFocusKey()
        {
            ConsoleKeyReader repeat = new ConsoleKeyReader();

            KeyboardState first = new KeyboardState(Keys.A);

            repeat.Update(first);

            Assert.AreEqual(Keys.A, repeat.GetCurrentKey());

            KeyboardState second = new KeyboardState(Keys.A, Keys.B);

            repeat.Update(second);

            Assert.AreEqual(Keys.B, repeat.GetCurrentKey());

            KeyboardState third = new KeyboardState(Keys.A);

            for (int i = 0; i < 100; ++i)
            {
                repeat.Update(third);
                Assert.AreEqual(Keys.None, repeat.GetCurrentKey());
            }

            KeyboardState last = new KeyboardState(Keys.A, Keys.B);

            repeat.Update(last);

            Assert.AreEqual(Keys.B, repeat.GetCurrentKey());
        }

        [Test]
        public void TestModifierState()
        {
            ConsoleKeyReader repeat = new ConsoleKeyReader();

            KeyboardState first = new KeyboardState(Keys.LeftShift);
            KeyboardState last = new KeyboardState(Keys.LeftShift, Keys.A);

            repeat.Update(first);

            Assert.AreEqual(KeyModifiers.Shift, repeat.GetModifiers());

            repeat.Update(last);

            Assert.AreEqual(KeyModifiers.Shift, repeat.GetModifiers());
            Assert.AreEqual(Keys.A, repeat.GetCurrentKey());
            
        }
    }
}
