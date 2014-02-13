using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gravitymania.input;

namespace gravitymania.console
{
    public class ConsoleInputLine
    {
        public KeyMap CurrentKeyMap { get; set; }

        public int CaretPosition
        {
            get { return _caretPosition; }
            set { _caretPosition = Math.Max(0, Math.Min(value, Text.Length)); }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value ?? ""; CaretPosition = _caretPosition; }
        }

        public ConsoleInputLine()
        {
            CurrentKeyMap = KeyMap.USKeyboard;
            Text = "";
            CaretPosition = 0;
        }

        public void Update(KeyModifiers modifiers, Keys inputKey)
        {
            switch (inputKey)
            {
                case Keys.Home:
                    CaretPosition = 0;
                    break;
                case Keys.End:
                    CaretPosition = Text.Length;
                    break;
                case Keys.Left:
                    --CaretPosition;
                    break;
                case Keys.Right:
                    ++CaretPosition;
                    break;
                case Keys.Back:
                    this.BackspaceCharAtCaret();
                    break;
                case Keys.Delete:
                    this.DeleteCharAtCaret();
                    break;
                case Keys.Space:
                    this.InsertSpaceAtCaret();
                    break;
                default:
                    this.InsertCharAtCaret(CurrentKeyMap.GetCharacter(modifiers, inputKey));
                    break;
            }
        }

        private void DeleteCharAtCaret()
        {
            if (CaretPosition < Text.Length)
            {
                Text = Text.Remove(CaretPosition, 1);
            }
        }

        private void BackspaceCharAtCaret()
        {
            if (CaretPosition > 0)
            {
                --CaretPosition;
                Text = Text.Remove(CaretPosition, 1);
            }
        }

        private void InsertSpaceAtCaret()
        {
            Text = Text.Insert(CaretPosition, " ");
            ++CaretPosition;
        }

        private void InsertCharAtCaret(char value)
        {
            if (!Char.IsControl(value))
            {
                Text = Text.Insert(CaretPosition, value.ToString());
                ++CaretPosition;
            }
        }

        private int _caretPosition;
        private string _text;
    }
}
