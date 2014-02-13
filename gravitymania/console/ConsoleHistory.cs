using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.math;

namespace gravitymania.console
{
    public class ConsoleHistory
    {
        public int HistorySize { get { return allHistory.Count; } }
        public int CommandBufferSize { get { return pastCommands.Count; } }

        public int CurrentCommand
        {
            get { return _currentCommand; }
            set
            {
                _currentCommand = NumTools.Clamp(value, 0, pastCommands.Count);
            }
        }

        public int HistoryLocation
        {
            get { return _historyLocation; }
            set
            {
                _historyLocation = NumTools.Clamp(value, 0, (allHistory.Count - HistoryWindowSize));
            }
        }

        public int HistoryWindowSize 
        {
            get { return _historyWindowSize; }
            set { _historyWindowSize = Math.Max(0, value); }
        }

        public ConsoleHistory()
        {
            _currentCommand = 0;
            _historyLocation = 0;
            _historyWindowSize = 1;
            allHistory = new List<string>();
            pastCommands = new List<string>();
        }

        public string GetCurrentCommand()
        {
            if (CurrentCommand >= pastCommands.Count)
            {
                return "";
            }
            else
            {
                return pastCommands[CurrentCommand];
            }
        }

        public IEnumerable<string> GetHistoryWindow()
        {
            int startPos = (int)Math.Max(0, HistoryLocation - HistoryWindowSize);
            return allHistory.GetRange(HistoryLocation, Math.Min(allHistory.Count - HistoryLocation, HistoryWindowSize));
        }

        public void ScrollToMostRecent()
        {
            HistoryLocation = allHistory.Count - HistoryWindowSize;
        }

        public void InsertEcho(string echoString)
        {
            allHistory.AddRange(echoString.Split('\n'));
            ScrollToMostRecent();
        }

        public void InsertCommand(string commandString)
        {
            pastCommands.Add(commandString);
            CurrentCommand = pastCommands.Count;
            InsertEcho("> " + commandString);
        }

        public void ClearEchoHistory()
        {
            allHistory.Clear();
            ScrollToMostRecent();
        }

        private int _currentCommand;
        private int _historyLocation;
        private int _historyWindowSize;

        private List<string> allHistory;
        private List<string> pastCommands;
    }
}
