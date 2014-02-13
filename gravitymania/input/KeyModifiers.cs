using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{
    public enum KeyModifiers : byte
    {
        None = 0x00,
        Shift = 0x01,
        Ctrl = 0x02,
        Alt = 0x04,
    }

    public static class KeyModifiersMethods
    {
        public static KeyModifiers GetModifiers(params Keys[] keys)
        {
            KeyModifiers result = KeyModifiers.None;

            foreach (Keys k in keys)
            {
                // This does not detect if caps lock is down, deal with it.  I have never found a conceivable use
                // for caps lock in any situation, so bite me if you miss it
                if (k == Keys.LeftShift || k == Keys.RightShift)
                {
                    result |= KeyModifiers.Shift;
                }
                else if (k == Keys.LeftControl || k == Keys.RightControl)
                {
                    result |= KeyModifiers.Ctrl;
                }
                else if (k == Keys.LeftAlt || k == Keys.RightAlt)
                {
                    result |= KeyModifiers.Alt;
                }
            }

            return result;
        }
    }
}
