using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.console
{
    public interface ConsoleExecutor
    {
        bool RunCommand(string inputLine);
    }
}
