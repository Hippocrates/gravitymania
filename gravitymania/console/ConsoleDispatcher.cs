using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.console
{
    public class ConsoleDispatcher
    {
        private List<ConsoleExecutor> Dispatchers;

        public ConsoleDispatcher()
        {
            Dispatchers = new List<ConsoleExecutor>();
        }

        public void PushDispatcher(ConsoleExecutor toAdd)
        {
            Dispatchers.Add(toAdd);
        }

        public ConsoleExecutor PopDispatcher()
        {
            ConsoleExecutor removed = null;
            if (Dispatchers.Count > 0)
            {
                int lastItem = Dispatchers.Count - 1;
                removed = Dispatchers[lastItem];
                Dispatchers.RemoveAt(lastItem);
            }

            return removed;
        }

        public bool RemoveDispatcher(ConsoleExecutor toRemove)
        {
            return Dispatchers.Remove(toRemove);
        }

        public void RunDispatchers(string command)
        {
            bool completed = false;

            for (int i = Dispatchers.Count - 1; i >= 0 && !completed; --i)
            {
                completed = Dispatchers[i].RunCommand(command);
            }
        }
    }
}
