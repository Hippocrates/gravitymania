using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace gravitymania.input
{
    public abstract class RawKey
    {
		/// <summary>
		/// Finds the appropriate 'key' given the name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
        public static RawKey Find(string name)
        {
            return KeyboardKey.FindKeyboardKey(name) as RawKey ??
                X360PadKey.FindX360PadKey(name) as RawKey ??
				MouseKey.FindMouseKey(name) as RawKey ?? 
                JoypadDigitalKey.FindJoypadDigitalKey(name) as RawKey ??
                JoypadAnalogKey.FindJoypadAnalogKey(name) as RawKey ??
                null;
        }

		public virtual bool IsAnalog()
		{
			return false;
		}
    }

    public class KeyboardKey : RawKey, IEquatable<KeyboardKey>
    {
		public static readonly IEnumerable<KeyboardKey> KeyList = BuildKeyList().ToArray();
        private static KeyMap CurrentKeyMap = KeyMap.USKeyboard;

        public Keys Key { get; private set; }

        public KeyboardKey(Keys key)
        {
            Key = key;
        }

        public static KeyboardKey FindKeyboardKey(string name)
        {
            Keys parseResult;
            if (name.Length > 0 && !char.IsDigit(name[0]) && Enum.TryParse<Keys>(name, true, out parseResult))
            {
                return new KeyboardKey(parseResult);
            }
            else if (name.Length == 1)
            {
                KeymapKey k = CurrentKeyMap.GetKeymapKey(name[0]);
                return (k != null) ? new KeyboardKey(k.Key) : null;
            }
            else
            {
                return null;
            }
        }

		private static IEnumerable<KeyboardKey> BuildKeyList()
		{
			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				yield return new KeyboardKey(key);
			}
		}

        public override string ToString()
        {
            return Key.ToString();
        }

		public bool Equals(KeyboardKey other)
		{
			return this.Key == other.Key;
		}
	}

	// This treats all xbox360 keys as digital inputs, we may want analogs eventually, but for now this works
    public class X360PadKey : RawKey, IEquatable<X360PadKey>
    {
		public static readonly IEnumerable<X360PadKey> KeyList = BuildKeyList().ToArray();
		public static readonly IEnumerable<Buttons> Analogs = new Buttons[] { Buttons.LeftTrigger, Buttons.RightTrigger, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp, Buttons.RightThumbstickDown };

        public const string X360Tag = "XPad";
        private const string PlayerIndexGroup = "controllerId";
        private const string KeynameGroup = "keyname";
        private static readonly Regex XPadRegex = new Regex("^" + X360Tag + "(?<" + PlayerIndexGroup + ">([0123]))\\.(?<" + KeynameGroup + ">(.+))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
		private static IEnumerable<X360PadKey> BuildKeyList()
		{
			foreach (PlayerIndex player in Enum.GetValues(typeof(PlayerIndex)))
			{
				foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
				{
					yield return new X360PadKey(player, button);
				}
			}
		}

        public Buttons Button { get; private set; }
        public PlayerIndex Controller { get; private set; }

        public X360PadKey(PlayerIndex controller, Buttons button)
        {
            Controller = controller;
            Button = button;
        }

        public override string ToString()
        {
            return X360Tag + ((int)Controller) + "." + Button.ToString();
        }

        public static X360PadKey FindX360PadKey(string name)
        {
            Match matches = XPadRegex.Match(name);

            if (matches == Match.Empty || matches.Groups[PlayerIndexGroup].Captures.Count != 1 || matches.Groups[KeynameGroup].Captures.Count != 1)
            {
                return null;
            }
            
            int controllerIndex;
            if (!int.TryParse(matches.Groups[PlayerIndexGroup].Captures[0].Value, out controllerIndex))
            {
                return null;
            }

			Buttons button;
            if (char.IsDigit(matches.Groups[KeynameGroup].Captures[0].Value[0]) || !Enum.TryParse<Buttons>(matches.Groups[KeynameGroup].Captures[0].Value, true, out button))
            {
                return null;
            }

            return new X360PadKey((PlayerIndex)controllerIndex, button);
        }

		public bool Equals(X360PadKey other)
		{
			return this.Controller == other.Controller && this.Button == other.Button;
		}

		public override bool IsAnalog()
		{
			if (Analogs.Contains(this.Button))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
    }

	public class MouseKey : RawKey, IEquatable<MouseKey>
	{
		public static readonly IEnumerable<MouseKey> KeyList = BuildKeyList().ToArray();

		private static readonly string MouseTag = "Mouse";
		private static readonly string ButtonGroup = "Button";
		private static readonly Regex MouseKeyRegex = new Regex("^" + MouseTag + "(?<" + ButtonGroup + ">(.+))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public MouseButton Button { get; private set; }

		public MouseKey(MouseButton button)
		{
			Button = button;
		}

		public static MouseKey FindMouseKey(string name)
		{
			Match matches = MouseKeyRegex.Match(name);

			if (matches == Match.Empty || matches.Groups[ButtonGroup].Captures.Count != 1)
			{
				return null;
			}

			MouseButton outButton;
			if (!Enum.TryParse<MouseButton>(matches.Groups[ButtonGroup].Captures[0].Value, out outButton))
			{
				return null;
			}

			return new MouseKey(outButton);
		}

		private static IEnumerable<MouseKey> BuildKeyList()
		{
			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
			{
				yield return new MouseKey(button);
			}
		}

		public override string ToString()
		{
			return Button.ToString();
		}

		public bool Equals(MouseKey other)
		{
			return this.Button == other.Button;
		}
	}

	public enum MouseButton
	{
		Left,
		Right,
		Middle,
		X1,
		X2,
	}

    /// <summary>
    /// Unfortunately these have proven to be fruitless, as the directx for .net libraries just plain don't work
    /// I'm looking into other solutions, but for now only xinput will be availiable (which is crap the x360ce hangs
    /// when getting player indices higher than One
    /// </summary>

    static class JoypadMatchHelper
    {
        public const string PadTag = "Pad";
        public const string ControllerIndexGroup = "controllerIndex";
        public const string ControllerMatch = "^" + PadTag + "(?<" + ControllerIndexGroup + ">(\\d))\\.";
    }

    public class JoypadDigitalKey : RawKey
    {
        private const string ButtonTag = "B";
        private const string ButtonIndexGroup = "buttonIndex";
        private static readonly Regex PadRegex = new Regex(JoypadMatchHelper.ControllerMatch + ButtonTag + "(?<" + ButtonIndexGroup + ">(\\d\\d?))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public int ButtonIndex { get; private set; }
        public int ControllerIndex { get; private set; }

        public JoypadDigitalKey(int controller, int button)
        {
            ControllerIndex = controller;
            ButtonIndex = button;
        }

        public static JoypadDigitalKey FindJoypadDigitalKey(string name)
        {
            Match matches = PadRegex.Match(name);

            if (matches == Match.Empty || matches.Groups[JoypadMatchHelper.ControllerIndexGroup].Captures.Count != 1 || matches.Groups[ButtonIndexGroup].Captures.Count != 1)
            {
                return null;
            }

            int controllerIndex;
            if (!int.TryParse(matches.Groups[JoypadMatchHelper.ControllerIndexGroup].Captures[0].Value, out controllerIndex) || controllerIndex < 0 || controllerIndex > 15)
            {
                return null;
            }

            int buttonIndex;
            if (!int.TryParse(matches.Groups[ButtonIndexGroup].Captures[0].Value, out buttonIndex) || buttonIndex < 0 || buttonIndex > 31)
            {
                return null;
            }

            return new JoypadDigitalKey(controllerIndex, buttonIndex);
        }

        public override string ToString()
        {
            return JoypadMatchHelper.PadTag + ControllerIndex + "." + ButtonTag + ButtonIndex;
        }
    }

    public class JoypadAnalogKey : RawKey
    {
        private const string AxisNameGroup = "buttonIndex";
        private const string AxisDirectionGroup = "axisDirection";
        private static readonly Regex PadRegex = new Regex(JoypadMatchHelper.ControllerMatch + "(?<" + AxisNameGroup + ">([XYZUV])).$", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        public JoypadAnalogAxis Axis { get; private set; }
        public JoypadAnalogDirection Direction { get; private set; } 
        public int ControllerIndex { get; private set; }

        public JoypadAnalogKey(int controller, JoypadAnalogAxis axis, JoypadAnalogDirection direction)
        {
            ControllerIndex = controller;
            Axis = axis;
            Direction = direction;
        }

        public static JoypadAnalogKey FindJoypadAnalogKey(string name)
        {
            Match matches = PadRegex.Match(name);

            if (matches == Match.Empty || matches.Groups[JoypadMatchHelper.ControllerIndexGroup].Captures.Count != 1 || matches.Groups[AxisNameGroup].Captures.Count != 1 || matches.Groups[AxisDirectionGroup].Captures.Count == 1)
            {
                return null;
            }

            int controllerIndex;
            if (!int.TryParse(matches.Groups[JoypadMatchHelper.ControllerIndexGroup].Captures[0].Value, out controllerIndex) || controllerIndex < 0 || controllerIndex > 15)
            {
                return null;
            }

            JoypadAnalogAxis axisId;
            if (char.IsDigit(matches.Groups[AxisNameGroup].Captures[0].Value[0]) || !Enum.TryParse<JoypadAnalogAxis>(matches.Groups[AxisNameGroup].Captures[0].Value, true, out axisId))
            {
                return null;
            }

            char lastChar = name.Last();

            if (lastChar != '-' && lastChar != '+')
            {
                return null;
            }

            JoypadAnalogDirection direction = (lastChar == '-') ? JoypadAnalogDirection.Negative : JoypadAnalogDirection.Positive;

            return new JoypadAnalogKey(controllerIndex, axisId, direction);
        }

        public override string ToString()
        {
            return JoypadMatchHelper.PadTag + ControllerIndex + "." + Axis + (Direction == JoypadAnalogDirection.Negative ? "-" : "+");
        }
    }

    public enum JoypadAnalogDirection
    {
        Negative,
        Positive,
    }

    public enum JoypadAnalogAxis
    {
        X,
        Y,
        Z,
        U,
        V,
    }
}
