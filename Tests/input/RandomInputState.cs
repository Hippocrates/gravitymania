using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gravitymania.input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace gravitymaniaTest.input
{
	public static class RandomInputState
	{
		public static InputState RandomState(Random rand)
		{
			List<Keys> pressedKeys = new List<Keys>();

			foreach (var k in KeyboardKey.KeyList)
			{
				if (rand.Next(2) == 0)
				{
					pressedKeys.Add(k.Key);
				}
			}

			KeyboardState kState = new KeyboardState(pressedKeys.ToArray());

			int mouseX = rand.Next(640);
			int mouseY = rand.Next(480);
			int wheel = rand.Next();

			List<MouseButton> pressedButtons = new List<MouseButton>();

			foreach (MouseButton k in Enum.GetValues(typeof(MouseButton)))
			{
				if (rand.Next(2) == 0)
				{
					pressedButtons.Add(k);
				}
			}

			MouseState mState = InputUtil.MakeMouseState(mouseX: mouseX, mouseY: mouseY, wheel: wheel, buttons: pressedButtons.ToArray());

			GamePadState[] padStates = Enum.GetValues(typeof(PlayerIndex)).Cast<PlayerIndex>().Select(i => MakePadState(rand, i)).ToArray();

			return new InputState(kState, mState, padStates);
		}

		private static ButtonState MakeButtonState(Random rand)
		{
			return rand.Next(2) == 0 ? ButtonState.Pressed : ButtonState.Released;
		}

		private static GamePadState MakePadState(Random rand, PlayerIndex player)
		{
			Vector2 leftStick = RandomStickPos(rand);
			Vector2 rightStick = RandomStickPos(rand);
			float leftTrigger = (float) rand.NextDouble();
			float rightTrigger = (float)rand.NextDouble();

			List<Buttons> pressed = new List<Buttons>();

			foreach (Buttons b in Enum.GetValues(typeof(Buttons)))
			{
				if (!X360PadKey.Analogs.Contains(b))
				{
					if (rand.Next(2) == 0)
					{
						pressed.Add(b);
					}
				}
			}

			return new GamePadState(leftStick, rightStick, leftTrigger, rightTrigger, pressed.ToArray());
		}

		private static Vector2 RandomStickPos(Random rand)
		{
			return new Vector2((float)((rand.NextDouble() * 2.0) - 1.0), (float)((rand.NextDouble() * 2.0) - 1.0));
		}
	}
}
