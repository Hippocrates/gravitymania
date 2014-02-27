using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace gravitymania.input
{
	public class InputFrame<ButtonEnum> where ButtonEnum : struct, IConvertible
    {
		static InputFrame()
		{
			if (!typeof(ButtonEnum).IsEnum)
			{
				throw new ArgumentException("T must be an enum type");
			}
		}

		public static readonly int NumButtons = Enum.GetValues(typeof(ButtonEnum)).Length;

        private BitArray keys = new BitArray(NumButtons);

		public InputFrame(params ButtonEnum[] buttons)
        {
			foreach (ButtonEnum b in buttons)
            {
				keys.Set((int)(object)b, true);
            }
        }

        public void SetState(ButtonEnum b, bool val)
        {
            keys.Set((int)(object)b, val);
        }

		public bool IsDown(ButtonEnum b)
        {
			return keys.Get((int)(object)b);
        }
    }
}
