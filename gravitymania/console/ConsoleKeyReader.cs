using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gravitymania.input;

namespace gravitymania.console
{
    public class ConsoleKeyReader
    {
        public uint InitialKeyRepeat { get; set; }
        public uint HeldKeyRepeat { get; set; }

        public ConsoleKeyReader()
        {
            InitialKeyRepeat = DefaultInitialKeyRepeat;
            HeldKeyRepeat = DefaultHeldKeyRepeat;

            this.lastState = new KeyboardState();
            this.focusKey = Keys.None;
            this.focusKeyHeldTime = 0;
            this.initialPress = false;
        }

        public void Update(KeyboardState keys)
        {
            Keys[] pressedKeys = keys.GetPressedKeys();

            KeyModifiers mods = KeyModifiersMethods.GetModifiers(pressedKeys);

            Keys[] diff = pressedKeys.Except(lastState.GetPressedKeys()).ToArray();

            if (diff.Length == 0 && pressedKeys.Contains(this.focusKey))
            {
                ++focusKeyHeldTime;

                if ((initialPress && focusKeyHeldTime >= InitialKeyRepeat) || (!initialPress && focusKeyHeldTime >= HeldKeyRepeat))
                {
                    focusKeyHeldTime = 0;
                    initialPress = false;
                }
            }
            else if (diff.Length == 1)
            {
                SetFocusKey(diff[0]);
            }
            else
            {
                SetFocusKey(Keys.None);
            }

            this.lastState = keys;
        }

        public Keys GetCurrentKey()
        {
            return this.focusKeyHeldTime == 0 ? this.focusKey : Keys.None;
        }

        public bool IsInitialPress()
        {
            return this.initialPress;
        }

        public KeyModifiers GetModifiers()
        {
            return KeyModifiersMethods.GetModifiers(this.lastState.GetPressedKeys());
        }

        private void SetFocusKey(Keys key)
        {
            this.focusKey = key;
            this.focusKeyHeldTime = 0;
            this.initialPress = true;
        }

        private Boolean isControlKey(Keys key)
        {
            return controlKeys.Contains(key);
        }

        private HashSet<Keys> controlKeys = new HashSet<Keys>()
        {
            Keys.None,
            Keys.Back,
            Keys.Delete,
            Keys.Tab,
            Keys.Home,
            Keys.End,
            Keys.Up,
            Keys.Down,
            Keys.Right,
            Keys.Left,
            Keys.PageDown,
            Keys.PageUp,
            Keys.Pause,
            Keys.CapsLock,
            Keys.Scroll,
            Keys.NumLock,
            Keys.Escape,
            Keys.F1,
            Keys.F2,
            Keys.F3,
            Keys.F4,
            Keys.F5,
            Keys.F6,
            Keys.F7,
            Keys.F8,
            Keys.F9,
            Keys.F10,
            Keys.F11,
            Keys.F12,
        };

        private KeyboardState lastState;
        private Keys focusKey;
        private uint focusKeyHeldTime;
        private bool initialPress;

        private const uint DefaultInitialKeyRepeat = 25;
        private const uint DefaultHeldKeyRepeat = 2;
    }
}
