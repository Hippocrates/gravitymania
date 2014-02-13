using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.console
{
    public static class ConsoleParser
    {
        public static string[] ParseCommandLine(string line)
        {
            List<string> tokens = new List<string>();
            StringBuilder currentToken = new StringBuilder();

            bool escaped = false;
            bool inSingleQuotes = false;
            bool inDoubleQuotes = false;

            for (int i = 0; i < line.Length; ++i)
            {
                char current = line[i];

                if (escaped)
                {
                    switch (current)
                    {
                    case 'n':
                        currentToken.Append('\n');
                        break;
                    case 't':
                        currentToken.Append('\t');
                        break;
                    case '\\':
                        currentToken.Append('\\');
                        break;
                    case '"':
                        currentToken.Append('"');
                        break;
                    case '\'':
                        currentToken.Append('\'');
                        break;
                    case ' ':
                        currentToken.Append(' ');
                        break;
                    default:
                        throw new Exception("Malformed console command: " + line);
                    }

                    escaped = false;
                }
                else
                {
                    if (current == '\\')
                    {
                        escaped = true;
                    }
                    else if (current == ' ' && !inSingleQuotes && !inDoubleQuotes)
                    {
                        if (currentToken.Length > 0)
                        {
                            tokens.Add(currentToken.ToString());
                            currentToken = new StringBuilder();
                        }
                    }
                    else if (current == '"' && !inSingleQuotes)
                    {
                        inDoubleQuotes = !inDoubleQuotes;
                    }
                    else if (current == '\'' && !inDoubleQuotes)
                    {
                        inSingleQuotes = !inSingleQuotes;
                    }
                    else
                    {
                        currentToken.Append(current);
                    }
                }
            }

            if (currentToken.Length > 0)
            {
                tokens.Add(currentToken.ToString());
            }

            return tokens.ToArray();
        }
    }
}
