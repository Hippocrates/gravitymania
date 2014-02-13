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
    class TestConsoleHistory
    {
        [Test]
        public void TestCreateHistory()
        {
            ConsoleHistory history = new ConsoleHistory();

            Assert.AreEqual(0, history.HistoryLocation);
            Assert.AreEqual(0, history.CurrentCommand);
            Assert.AreEqual(1, history.HistoryWindowSize);
            Assert.AreEqual(0, history.HistorySize);
            Assert.AreEqual(0, history.CommandBufferSize);
            Assert.AreEqual("", history.GetCurrentCommand());
        }

        [Test]
        public void TestEnterCommand()
        {
            ConsoleHistory history = new ConsoleHistory();

            string command = "some kind of command";

            history.InsertCommand(command);

            Assert.AreEqual(0, history.HistoryLocation);
            Assert.AreEqual(1, history.CurrentCommand);
            Assert.AreEqual(1, history.HistorySize);
            Assert.AreEqual(1, history.CommandBufferSize);
            Assert.AreEqual("", history.GetCurrentCommand());
            --history.CurrentCommand;
            Assert.AreEqual(command, history.GetCurrentCommand());

            string command2 = "some kind of command";

            history.InsertCommand(command2);

            Assert.AreEqual(1, history.HistoryLocation);
            Assert.AreEqual(2, history.CurrentCommand);
            Assert.AreEqual(2, history.HistorySize);
            Assert.AreEqual(2, history.CommandBufferSize);
            Assert.AreEqual("", history.GetCurrentCommand());
            --history.CurrentCommand;
            Assert.AreEqual(command2, history.GetCurrentCommand());
            --history.CurrentCommand;
            Assert.AreEqual(command, history.GetCurrentCommand());
        }

        [Test]
        public void TestEchoInformation()
        {
            ConsoleHistory history = new ConsoleHistory();
            
            string info = "some kind of information";

            history.InsertEcho(info);

            Assert.AreEqual(0, history.HistoryLocation);
            Assert.AreEqual(0, history.CurrentCommand);
            Assert.AreEqual(1, history.HistorySize);
            Assert.AreEqual(0, history.CommandBufferSize);
            Assert.AreEqual(info, history.GetHistoryWindow().First());

            string command = "some kind of command";

            history.InsertCommand(command);

            Assert.AreEqual(1, history.HistoryLocation);
            Assert.AreEqual(1, history.CurrentCommand);
            Assert.AreEqual(2, history.HistorySize);
            Assert.AreEqual(1, history.CommandBufferSize);

            history.HistoryWindowSize = 5;
            history.ScrollToMostRecent();

            IEnumerator<string> e = history.GetHistoryWindow().GetEnumerator();

            Assert.AreEqual(2, history.GetHistoryWindow().Count());
            
            e.MoveNext();
            Assert.AreEqual(info, e.Current);

            e.MoveNext();
            Assert.AreEqual("> " + command, e.Current);
        }

        [Test]
        public void TestHistoryScroll()
        {
            ConsoleHistory history = new ConsoleHistory();
            history.HistoryWindowSize = 5;

            for (int i = 0; i < 20; ++i)
            {
                history.InsertEcho("" + i);
            }

            Assert.AreEqual(15, history.HistoryLocation);
            Assert.AreEqual(20, history.HistorySize);
            Assert.AreEqual(0, history.CommandBufferSize);

            --history.HistoryLocation;

            Assert.AreEqual(14, history.HistoryLocation);

            for (int i = 0; i < 20; ++i)
            {
                ++history.HistoryLocation;
            }

            Assert.AreEqual(history.HistorySize - history.HistoryWindowSize, history.HistoryLocation);

            history.ClearEchoHistory();

            Assert.AreEqual(0, history.HistorySize);
            Assert.AreEqual(0, history.HistoryLocation);
        }
    }
}
