using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{
	public class InputEventManager
	{
		public event Action<KeyboardKey, bool, KeyboardState> KeyboardEvent;
		public event Action<MouseState> MouseMouseEvent;
        public event Action<int, MouseState> MouseWheelEvent;
		public event Action<MouseKey, bool, MouseState> MouseButtonEvent;

		private InputState PreviousState;
		private InputState CurrentState;

        public void Flush()
        {
            PreviousState = new InputState();
        }

		public void Update(InputState state)
		{
			PreviousState = CurrentState;
			CurrentState = state;

			foreach (KeyboardKey key in KeyboardKey.KeyList)
			{
				if (CurrentState.GetButtonState(key) != PreviousState.GetButtonState(key))
				{
					if (KeyboardEvent != null)
					{
						KeyboardEvent.Invoke(key, CurrentState.GetButtonState(key) == ButtonState.Pressed, CurrentState.Keys);
					}
				}
			}

			foreach (MouseKey key in MouseKey.KeyList)
			{
				if (CurrentState.GetButtonState(key) != PreviousState.GetButtonState(key))
				{
					if (MouseButtonEvent != null)
					{
						MouseButtonEvent.Invoke(key, CurrentState.GetButtonState(key) == ButtonState.Pressed, CurrentState.Mouse);
					}
				}
			}

            if (CurrentState.Mouse.ScrollWheelValue != PreviousState.Mouse.ScrollWheelValue)
            {
                if (MouseWheelEvent != null)
                {
                    MouseWheelEvent.Invoke(CurrentState.Mouse.ScrollWheelValue - PreviousState.Mouse.ScrollWheelValue, CurrentState.Mouse);
                }
            }

			if (CurrentState.Mouse.X != PreviousState.Mouse.X ||
				CurrentState.Mouse.Y != PreviousState.Mouse.Y)
			{
				if (MouseMouseEvent != null)
				{
					MouseMouseEvent.Invoke(CurrentState.Mouse);
				}
			}
		}

	}
}
