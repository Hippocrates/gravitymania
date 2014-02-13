using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using gravitymania.console;

namespace gravitymaniaTest.console
{
    class NotSoDumb : ConsoleExecutor
    {
        public bool PassThrough { get; set; }
        public List<string> Commands = new List<string>();

        public bool RunCommand(string inputLine)
        {
            Commands.Add(inputLine);
            return !PassThrough;
        }
    }

    [TestFixture]
    class TestAddRemoveDispatcher
    {
        [Test]
        public void AddRemoveDispatcher()
        {
            ConsoleDispatcher console = new ConsoleDispatcher();

            ConsoleExecutor dummy = new DummyConsoleExecutor();

            console.PushDispatcher(dummy);

            Assert.IsTrue(console.RemoveDispatcher(dummy));

            NotSoDumb a = new NotSoDumb() { PassThrough = false };
            NotSoDumb b = new NotSoDumb() { PassThrough = true };
            NotSoDumb c = new NotSoDumb() { PassThrough = false };

            console.PushDispatcher(a);

            string first = "test1";

            console.RunDispatchers(first);

            Assert.AreEqual(1, a.Commands.Count);
            Assert.AreEqual(first, a.Commands[0]);

            string second = "test2";

            console.RunDispatchers(second);

            Assert.AreEqual(2, a.Commands.Count);
            Assert.AreEqual(second, a.Commands[1]);

            console.PushDispatcher(b);

            string third = "test3";

            console.RunDispatchers(third);

            Assert.AreEqual(3, a.Commands.Count);
            Assert.AreEqual(third, a.Commands[2]);
            Assert.AreEqual(1, b.Commands.Count);
            Assert.AreEqual(third, b.Commands[0]);

            console.RemoveDispatcher(a);

            string fourth = "test4";

            console.RunDispatchers(fourth);

            Assert.AreEqual(3, a.Commands.Count);
            Assert.AreEqual(2, b.Commands.Count);
            Assert.AreEqual(fourth, b.Commands[1]);

            console.PushDispatcher(c);

            string fifth = "test5";

            console.RunDispatchers(fifth);

            Assert.AreEqual(2, b.Commands.Count);
            Assert.AreEqual(1, c.Commands.Count);
            Assert.AreEqual(fifth, c.Commands[0]);
        }
    }
}
