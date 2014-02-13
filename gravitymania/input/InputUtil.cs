using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{
	public static class InputUtil
	{
		public static MouseState MakeMouseState(int mouseX = 0, int mouseY = 0, int wheel = 0, params MouseButton[] buttons)
		{
			ButtonState lb = ButtonState.Released;
			ButtonState rb = ButtonState.Released;
			ButtonState mb = ButtonState.Released;
			ButtonState x1b = ButtonState.Released;
			ButtonState x2b = ButtonState.Released;

			foreach (MouseButton b in buttons)
			{
				switch (b)
				{
					case MouseButton.Left:
						lb = ButtonState.Pressed;
						break;
					case MouseButton.Right:
						rb = ButtonState.Pressed;
						break;
					case MouseButton.Middle:
						mb = ButtonState.Pressed;
						break;
					case MouseButton.X1:
						x1b = ButtonState.Pressed;
						break;
					case MouseButton.X2:
						x2b = ButtonState.Pressed;
						break;
				}
			}

			return new MouseState(mouseX, mouseY, wheel, lb, mb, rb, x1b, x2b);
		}
	}
}
