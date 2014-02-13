using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{
    public class KeymapKey : IEquatable<KeymapKey>
    {
        public KeyModifiers Modifiers { get; private set; }
        public Keys Key { get; private set; }

        public KeymapKey(Keys key)
            : this(0x00, key)
        {
        }

        public KeymapKey(KeyModifiers modifiers, Keys key)
        {
            Modifiers = modifiers;
            Key = key;
        }

        public bool Equals(KeymapKey other)
        {
            return (Modifiers == other.Modifiers) && (Key == other.Key);
        }

        public override int GetHashCode()
        {
            return (byte)Modifiers + ((int)Key * 256);
        }
    }
}
