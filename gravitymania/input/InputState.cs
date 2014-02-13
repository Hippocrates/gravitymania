using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace gravitymania.input
{
    public struct InputState
    {
		public readonly KeyboardState Keys;
		public readonly MouseState Mouse;
		public readonly GamePadState[] Pads;

        private const float AnalogDeadZone = 0.01f;

        public static InputState ReadCurrentState()
        {
            return new InputState(Keyboard.GetState(), Microsoft.Xna.Framework.Input.Mouse.GetState(), Enum.GetValues(typeof(PlayerIndex)).Cast<PlayerIndex>().Select(p => GamePad.GetState(p)).ToArray());
        }

        public InputState(KeyboardState keys = new KeyboardState(), MouseState mouse = new MouseState(), params GamePadState[] pads)
        {
            Keys = keys;
            Mouse = mouse;
            Pads = pads.ToArray();
        }

		public ButtonState GetButtonState(RawKey b)
		{
			return this.GetAnalogState(b) > AnalogDeadZone ? ButtonState.Pressed : ButtonState.Released;
		}

        public float GetAnalogState(RawKey b)
        {
            if (b is KeyboardKey)
            {
                KeyboardKey k = b as KeyboardKey;

                return Keys.IsKeyDown(k.Key) ? 1.0f : 0.0f;
            }
            else if (b is X360PadKey)
            {
                X360PadKey g = b as X360PadKey;

                GamePadState currentPad = Pads[(int)g.Controller];

                switch (g.Button)
                {
                case Buttons.A:
                    return currentPad.Buttons.A == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.B:
                    return currentPad.Buttons.B == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.X:
                    return currentPad.Buttons.X == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.Y:
                    return currentPad.Buttons.Y == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.Back:
                    return currentPad.Buttons.Back == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.Start:
                    return currentPad.Buttons.Start == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.RightShoulder:
                    return currentPad.Buttons.RightShoulder == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.LeftShoulder:
                    return currentPad.Buttons.LeftShoulder == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.RightTrigger:
                    return currentPad.Triggers.Right;
				case Buttons.LeftTrigger:
                    return currentPad.Triggers.Left;
				case Buttons.RightStick:
                    return currentPad.Buttons.RightStick == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.LeftStick:
                    return currentPad.Buttons.LeftStick == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.DPadUp:
                    return currentPad.DPad.Up == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.DPadDown:
                    return currentPad.DPad.Down == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.DPadLeft:
                    return currentPad.DPad.Left == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.DPadRight:
                    return currentPad.DPad.Right == ButtonState.Pressed ? 1.0f : 0.0f;
				case Buttons.LeftThumbstickUp:
                    return -currentPad.ThumbSticks.Left.Y;
				case Buttons.LeftThumbstickDown:
                    return currentPad.ThumbSticks.Left.Y;
				case Buttons.LeftThumbstickLeft:
                    return -currentPad.ThumbSticks.Left.X;
				case Buttons.LeftThumbstickRight:
                    return currentPad.ThumbSticks.Left.X;
				case Buttons.RightThumbstickUp:
                    return -currentPad.ThumbSticks.Right.Y;
				case Buttons.RightThumbstickDown:
                    return currentPad.ThumbSticks.Right.Y;
				case Buttons.RightThumbstickLeft:
                    return -currentPad.ThumbSticks.Right.X;
				case Buttons.RightThumbstickRight:
                    return currentPad.ThumbSticks.Right.X;
                default:
                    throw new Exception("Error, unhandled joypad button: " + g.ToString());
                }
            }
			else if (b is MouseKey)
			{
				MouseKey m = b as MouseKey;

				switch (m.Button)
                {
					case MouseButton.Left:
						return Mouse.LeftButton == ButtonState.Pressed ? 1.0f : 0.0f;
					case MouseButton.Right:
						return Mouse.RightButton == ButtonState.Pressed ? 1.0f : 0.0f;
					case MouseButton.Middle:
						return Mouse.MiddleButton == ButtonState.Pressed ? 1.0f : 0.0f;
					case MouseButton.X1:
						return Mouse.XButton1 == ButtonState.Pressed ? 1.0f : 0.0f;
					case MouseButton.X2:
						return Mouse.XButton2 == ButtonState.Pressed ? 1.0f : 0.0f;
					default:
						throw new Exception("Error, unhandled mouse button: " + m.ToString());
				}
			}
			/*else if (b is JoypadDigitalKey)
			{
				JoypadDigitalKey d = b as JoypadDigitalKey;

				if (Joysticks.Length > d.ControllerIndex)
				{
					return ((Joysticks[d.ControllerIndex].GetButtons()[d.ButtonIndex] & 0x80) == 0x80) ? ButtonState.Pressed : ButtonState.Released;
				}
				else
				{
					return ButtonState.Released;
				}
			}
			else if (b is JoypadAnalogKey)
			{
				JoypadAnalogKey a = b as JoypadAnalogKey;

				if (Joysticks.Length > a.ControllerIndex)
				{
					float value = GetAxisValue(Joysticks[a.ControllerIndex], a.Axis);

					if (a.Direction == JoypadAnalogDirection.Negative)
					{
						return value < -AnalogDeadZone ? ButtonState.Pressed : ButtonState.Released;
					}
					else
					{
						return value > AnalogDeadZone ? ButtonState.Pressed : ButtonState.Released;
					}
				}
				else
				{
					return ButtonState.Released;
				}
			}*/
			else
			{
				throw new Exception("Error, unhandled input device: " + b.GetType().ToString());
			}
        }
        /*
        private static float GetAxisValue(Microsoft.DirectX.DirectInput.JoystickState state, JoypadAnalogAxis axis)
        {
            int value = 0;
            switch(axis)
            {
            case JoypadAnalogAxis.X:
                value = state.X;
                break;
            case JoypadAnalogAxis.Y:
                value = state.Y;
                break;
            case JoypadAnalogAxis.Z:
                value = state.Z;
                break;
            case JoypadAnalogAxis.U:
                value = state.GetSlider()[0];
                break;
            case JoypadAnalogAxis.V:
                value = state.GetSlider()[1];
                break;
            default:
                throw new Exception("Error, unhandled axis: " + axis.ToString());
            }

            return ((float)value) / ((float)DX8Joypad.JoypadAxisRange);
        }*/
    }
}
