using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.input
{

    public enum InputPlayerIndex
    {
        One = 0,
        Two = 1,
        Neither = 2,
    }

    public enum EventCode
    {
        None = 0,
        Left,
        Right,
        Jump,
    }

    public class EventKey : IEquatable<EventKey>
    {
        public EventCode Code;
        public InputPlayerIndex Player;

        public EventKey(EventCode code)
        {
            Code = code;
            Player = InputPlayerIndex.Neither;
        }

        public EventKey(EventCode code, InputPlayerIndex player)
        {
            Code = code;
            Player = player;
        }

        public override int GetHashCode()
        {
            return (((int)Player) * Enum.GetNames(typeof(EventCode)).Length) + (int)Code;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EventKey);
        }

        public bool Equals(EventKey other)
        {
            if (other != null)
            {
                return Code == other.Code && Player == other.Player;
            }
            else
            {
                return false;
            }
        }
    }
}
