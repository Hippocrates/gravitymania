using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania;
using gravitymania.console;
using Microsoft.Xna.Framework.Input;

namespace gravitymaniaTest.console
{
    [TestFixture]
    class TestConsoleInputLine
    {
        [Test]
        public void TestBasicInputLine()
        {
            ConsoleInputLine line = new ConsoleInputLine();

            Assert.AreEqual(0, line.CaretPosition);
            Assert.AreEqual("", line.Text);
        }

        [Test]
        public void TestBasicInputControl()
        {
            ConsoleInputLine line = new ConsoleInputLine();
            ConsoleKeyReader reader = new ConsoleKeyReader();

            reader.Update(new KeyboardState(Keys.A));

            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("a", line.Text);
            Assert.AreEqual(1, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.LeftShift));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            reader.Update(new KeyboardState(Keys.LeftShift, Keys.B));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB", line.Text);
            Assert.AreEqual(2, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.LeftShift, Keys.D8));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*", line.Text);
            Assert.AreEqual(3, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.Home));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*", line.Text);
            Assert.AreEqual(0, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.End));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*", line.Text);
            Assert.AreEqual(3, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.LeftShift));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            reader.Update(new KeyboardState(Keys.LeftShift, Keys.D2));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*@", line.Text);
            Assert.AreEqual(4, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.Back));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*", line.Text);
            Assert.AreEqual(3, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.Left));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*", line.Text);
            Assert.AreEqual(2, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.R));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aBr*", line.Text);
            Assert.AreEqual(3, line.CaretPosition);


            reader.Update(new KeyboardState(Keys.Back));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("aB*", line.Text);
            Assert.AreEqual(2, line.CaretPosition);

            for (int i = 0; i < reader.InitialKeyRepeat; ++i)
            {
                reader.Update(new KeyboardState(Keys.Back));
                line.Update(reader.GetModifiers(), reader.GetCurrentKey());
            }

            Assert.AreEqual("a*", line.Text);
            Assert.AreEqual(1, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.Delete));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("a", line.Text);
            Assert.AreEqual(1, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.Delete));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("a", line.Text);
            Assert.AreEqual(1, line.CaretPosition);

            reader.Update(new KeyboardState(Keys.Back));
            line.Update(reader.GetModifiers(), reader.GetCurrentKey());

            Assert.AreEqual("", line.Text);
            Assert.AreEqual(0, line.CaretPosition);
        }
    }
}
