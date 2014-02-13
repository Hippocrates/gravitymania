using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.console
{
    public class DummyConsoleExecutor : ConsoleExecutor
    {
        public bool RunCommand(string inputLine)
        {
            // Kentucy do-nothing
            return false;
        }
    }
}
