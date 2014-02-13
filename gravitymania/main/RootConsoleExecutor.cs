using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.console;

namespace gravitymania.main
{
    public class RootConsoleExecutor : ConsoleExecutor
    {
        private GameRoot Root;
        private Dictionary<string, Action<string[]>> Commands;

        public RootConsoleExecutor(GameRoot root)
        {
            Root = root;

            Commands = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase)
            {
                { "startgame", this.RunGame },
                { "editmap", this.RunMapEdtior },
                { "exit", this.ExitGame },
                { "close", this.CloseConsole },
                { "clear", this.ClearConsole },
            };
        }

        private void Echo(string output)
        {
            Root.Console.PrintLine(output);
        }

        public bool RunCommand(string inputLine)
        {
            string[] args = ConsoleParser.ParseCommandLine(inputLine);

            if (args.Length > 0 && Commands.ContainsKey(args[0]))
            {
                Commands[args[0]].Invoke(args);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void RunGame(string[] input)
        {
            Root.SwitchToGame();
            Echo("Game Started Successfully.");
        }

        private void RunMapEdtior(string[] input)
        {
            Root.SwitchToEditor();
            Echo("Editor Initialized.");
        }

        private void ExitGame(string[] input)
        {
            Echo("Exiting...");
            Root.Exit();
        }

        private void CloseConsole(string[] input)
        {
            Root.Console.IsOpen = false;
        }

        private void ClearConsole(string[] input)
        {
            Root.Console.ClearScreen();
        }
    }
}
