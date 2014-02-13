using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{
    public class KeyMap
    {
        private KeyMap(Dictionary<KeymapKey, char> values)
        {
            Values = values;
            ReverseLookup = new Dictionary<char, KeymapKey>();

            Keys[] AmbiguousNumpadKeys = new Keys[]{ Keys.Divide, Keys.Add, Keys.Multiply, Keys.Subtract, Keys.Decimal };

            foreach (var pair in Values)
            {
                // make sure that we don't overwrite the normal keyboard keys with the numpad keys of the same character
                if (!pair.Key.Key.ToString().Contains("NumPad") && !AmbiguousNumpadKeys.Contains(pair.Key.Key))
                {
                    ReverseLookup[pair.Value] = pair.Key;
                }
            }
        }

        public char GetCharacter(Keys key)
        {
            return GetCharacter(KeyModifiers.None, key);
        }

        public char GetCharacter(KeyModifiers modifiers, Keys key)
        {
            char value = '\0';
            if (Values.TryGetValue(new KeymapKey(modifiers, key), out value))
            {
                return value;
            }
            else
            {
                return '\0';
            }
        }

        public KeymapKey GetKeymapKey(char c)
        {
            KeymapKey value = null;
            if (ReverseLookup.TryGetValue(c, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        private Dictionary<KeymapKey, char> Values;
        private Dictionary<char, KeymapKey> ReverseLookup;

        public static readonly KeyMap USKeyboard = new KeyMap(
            new Dictionary<KeymapKey, char>()
            {
                { new KeymapKey(Keys.A), 'a'},
                { new KeymapKey(KeyModifiers.Shift, Keys.A), 'A'},
                { new KeymapKey(Keys.B), 'b'},
                { new KeymapKey(KeyModifiers.Shift, Keys.B), 'B'},
                { new KeymapKey(Keys.C), 'c'},
                { new KeymapKey(KeyModifiers.Shift, Keys.C), 'C'},
                { new KeymapKey(Keys.D), 'd'},
                { new KeymapKey(KeyModifiers.Shift, Keys.D), 'D'},
                { new KeymapKey(Keys.E), 'e'},
                { new KeymapKey(KeyModifiers.Shift, Keys.E), 'E'},
                { new KeymapKey(Keys.F), 'f'},
                { new KeymapKey(KeyModifiers.Shift, Keys.F), 'F'},
                { new KeymapKey(Keys.G), 'g'},
                { new KeymapKey(KeyModifiers.Shift, Keys.G), 'G'},
                { new KeymapKey(Keys.H), 'h'},
                { new KeymapKey(KeyModifiers.Shift, Keys.H), 'H'},
                { new KeymapKey(Keys.I), 'i'},
                { new KeymapKey(KeyModifiers.Shift, Keys.I), 'I'},
                { new KeymapKey(Keys.J), 'j'},
                { new KeymapKey(KeyModifiers.Shift, Keys.J), 'J'},
                { new KeymapKey(Keys.K), 'k'},
                { new KeymapKey(KeyModifiers.Shift, Keys.K), 'K'},
                { new KeymapKey(Keys.L), 'l'},
                { new KeymapKey(KeyModifiers.Shift, Keys.L), 'L'},
                { new KeymapKey(Keys.M), 'm'},
                { new KeymapKey(KeyModifiers.Shift, Keys.M), 'M'},
                { new KeymapKey(Keys.N), 'n'},
                { new KeymapKey(KeyModifiers.Shift, Keys.N), 'N'},
                { new KeymapKey(Keys.O), 'o'},
                { new KeymapKey(KeyModifiers.Shift, Keys.O), 'O'},
                { new KeymapKey(Keys.P), 'p'},
                { new KeymapKey(KeyModifiers.Shift, Keys.P), 'P'},
                { new KeymapKey(Keys.Q), 'q'},
                { new KeymapKey(KeyModifiers.Shift, Keys.Q), 'Q'},
                { new KeymapKey(Keys.R), 'r'},
                { new KeymapKey(KeyModifiers.Shift, Keys.R), 'R'},
                { new KeymapKey(Keys.S), 's'},
                { new KeymapKey(KeyModifiers.Shift, Keys.S), 'S'},
                { new KeymapKey(Keys.T), 't'},
                { new KeymapKey(KeyModifiers.Shift, Keys.T), 'T'},
                { new KeymapKey(Keys.U), 'u'},
                { new KeymapKey(KeyModifiers.Shift, Keys.U), 'U'},
                { new KeymapKey(Keys.V), 'v'},
                { new KeymapKey(KeyModifiers.Shift, Keys.V), 'V'},
                { new KeymapKey(Keys.W), 'w'},
                { new KeymapKey(KeyModifiers.Shift, Keys.W), 'W'},
                { new KeymapKey(Keys.X), 'x'},
                { new KeymapKey(KeyModifiers.Shift, Keys.X), 'X'},
                { new KeymapKey(Keys.Y), 'y'},
                { new KeymapKey(KeyModifiers.Shift, Keys.Y), 'Y'},
                { new KeymapKey(Keys.Z), 'z'},
                { new KeymapKey(KeyModifiers.Shift, Keys.Z), 'Z'},
                { new KeymapKey(Keys.D0), '0'},
                { new KeymapKey(Keys.NumPad0), '0' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D0), ')'},
                { new KeymapKey(Keys.D1), '1'},
                { new KeymapKey(Keys.NumPad1), '1' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D1), '!'},
                { new KeymapKey(Keys.D2), '2'},
                { new KeymapKey(Keys.NumPad2), '2' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D2), '@'},
                { new KeymapKey(Keys.D3), '3'},
                { new KeymapKey(Keys.NumPad3), '3' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D3), '#'},
                { new KeymapKey(Keys.D4), '4'},
                { new KeymapKey(Keys.NumPad4), '4' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D4), '$'},
                { new KeymapKey(Keys.D5), '5'},
                { new KeymapKey(Keys.NumPad5), '5' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D5), '%'},
                { new KeymapKey(Keys.D6), '6'},
                { new KeymapKey(Keys.NumPad6), '6' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D6), '^'},
                { new KeymapKey(Keys.D7), '7'},
                { new KeymapKey(Keys.NumPad7), '7' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D7), '&'},
                { new KeymapKey(Keys.D8), '8'},
                { new KeymapKey(Keys.NumPad8), '8' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D8), '*'},
                { new KeymapKey(Keys.D9), '9'},
                { new KeymapKey(Keys.NumPad9), '9' },
                { new KeymapKey(KeyModifiers.Shift, Keys.D9), '('},
                { new KeymapKey(Keys.Divide), '/' },
                { new KeymapKey(Keys.Add), '+' },
                { new KeymapKey(Keys.Subtract), '-' },
                { new KeymapKey(Keys.Multiply), '*' },
                { new KeymapKey(Keys.Decimal), '.' },
                { new KeymapKey(Keys.OemMinus), '-' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemMinus), '_' },
                { new KeymapKey(Keys.OemPlus), '=' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemPlus), '+' },
                { new KeymapKey(Keys.OemTilde), '`' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemTilde), '~' },
                { new KeymapKey(Keys.OemOpenBrackets), '[' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemOpenBrackets), '{' },
                { new KeymapKey(Keys.OemCloseBrackets), ']' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemCloseBrackets), '}' },
                { new KeymapKey(Keys.OemComma), ',' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemComma), '<' },
                { new KeymapKey(Keys.OemPeriod), '.' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemPeriod), '>' },
                { new KeymapKey(Keys.OemQuestion), '/' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemQuestion), '?' },
                { new KeymapKey(Keys.OemPipe), '\\' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemPipe), '|' },
                { new KeymapKey(Keys.OemSemicolon), ';' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemSemicolon), ':' },
                { new KeymapKey(Keys.OemQuotes), '\'' },
                { new KeymapKey(KeyModifiers.Shift, Keys.OemQuotes), '"' },
            });
    }
}
