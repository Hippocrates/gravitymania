using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using gravitymania;
using gravitymania.console;
using Microsoft.Xna.Framework.Input;
using gravitymania.input;

namespace gravitymaniaTest.input
{
	public class DummyKeyboardLogger
	{
		public List<KeyboardKey> pressedKeys = new List<KeyboardKey>();
		public List<KeyboardKey> releasedKeys = new List<KeyboardKey>();

		public void KeyEvent(KeyboardKey key, bool pressed, KeyboardState state)
		{
			if (pressed)
			{
				pressedKeys.Add(key);
			}
			else
			{
				releasedKeys.Add(key);
			}
		}

		public void Clear()
		{
			pressedKeys.Clear();
			releasedKeys.Clear();
		}
	}

	public class DummyMouseMoveLogger
	{
		public List<Tuple<int, int>> MouseMoves = new List<Tuple<int, int>>();

		public void MouseMoveEvent(MouseState state)
		{
			MouseMoves.Add(new Tuple<int, int>(state.X, state.Y));
		}

		public void Clear()
		{
			MouseMoves.Clear();
		}
	}

	public class DummyMouseButtonLogger
	{
		public List<MouseKey> pressedKeys = new List<MouseKey>();
		public List<MouseKey> releasedKeys = new List<MouseKey>();

		public void KeyEvent(MouseKey key, bool pressed, MouseState state)
		{
			if (pressed)
			{
				pressedKeys.Add(key);
			}
			else
			{
				releasedKeys.Add(key);
			}
		}

		public void Clear()
		{
			pressedKeys.Clear();
			releasedKeys.Clear();
		}
	}

	[TestFixture]
	public class TestInputEventManager
	{
		[Test]
		public void TestBasicKeyPressEvent()
		{
			InputEventManager manager = new InputEventManager();

			Keys[] keys = new Keys[]{ Keys.B, Keys.A, Keys.C };

			InputState state = new InputState(keys: new KeyboardState(keys));

			DummyKeyboardLogger logger = new DummyKeyboardLogger();

			manager.KeyboardEvent += logger.KeyEvent;
			manager.Update(state);

			Assert.AreEqual(keys.Length, logger.pressedKeys.Count);
			foreach (var k in keys)
			{
				Assert.IsTrue(logger.pressedKeys.Contains(new KeyboardKey(k)));
			}

			Assert.IsEmpty(logger.releasedKeys);

			Keys[] k2 = new Keys[] { Keys.C, Keys.D };

			logger.Clear();
			state = new InputState(keys: new KeyboardState(k2));
			manager.Update(state);

			Assert.AreEqual(1, logger.pressedKeys.Count);
			Assert.IsTrue(new KeyboardKey(Keys.D).Equals(logger.pressedKeys[0]));

			Assert.AreEqual(2, logger.releasedKeys.Count);
			Assert.IsTrue(logger.releasedKeys.Contains(new KeyboardKey(Keys.A)));
			Assert.IsTrue(logger.releasedKeys.Contains(new KeyboardKey(Keys.B)));
		}

		[Test]
		public void TestBasicMouseMoveEvent()
		{
			InputEventManager manager = new InputEventManager();

			int mouseX = 123;
			int mouseY = 33;

			MouseState state = InputUtil.MakeMouseState(mouseX: mouseX, mouseY: mouseY);

			DummyMouseMoveLogger logger = new DummyMouseMoveLogger();

			manager.MouseMouseEvent += logger.MouseMoveEvent;

			manager.Update(new InputState(mouse: state));

			Assert.AreEqual(1, logger.MouseMoves.Count);
			Assert.AreEqual(mouseX, logger.MouseMoves[0].Item1);
			Assert.AreEqual(mouseY, logger.MouseMoves[0].Item2);

			logger.Clear();
			manager.Update(new InputState(mouse: state));

			Assert.IsEmpty(logger.MouseMoves);

			mouseX = 124;

			state = InputUtil.MakeMouseState(mouseX: mouseX, mouseY: mouseY);

			logger.Clear();
			manager.Update(new InputState(mouse: state));

			Assert.AreEqual(1, logger.MouseMoves.Count);
			Assert.AreEqual(mouseX, logger.MouseMoves[0].Item1);
			Assert.AreEqual(mouseY, logger.MouseMoves[0].Item2);

			mouseY = 31;

			state = InputUtil.MakeMouseState(mouseX: mouseX, mouseY: mouseY);

			logger.Clear();
			manager.Update(new InputState(mouse: state));

			Assert.AreEqual(1, logger.MouseMoves.Count);
			Assert.AreEqual(mouseX, logger.MouseMoves[0].Item1);
			Assert.AreEqual(mouseY, logger.MouseMoves[0].Item2);
		}

		[Test]
		public void TestBasicMouseButtonEvent()
		{
			InputEventManager manager = new InputEventManager();

			MouseButton[] buttons = new MouseButton[] { MouseButton.Left, MouseButton.Middle, MouseButton.X2 };

			InputState state = new InputState(mouse: InputUtil.MakeMouseState(buttons: buttons));

			DummyMouseButtonLogger logger = new DummyMouseButtonLogger();

			manager.MouseButtonEvent += logger.KeyEvent;
			manager.Update(state);

			Assert.AreEqual(buttons.Length, logger.pressedKeys.Count);
			foreach (var k in buttons)
			{
				Assert.IsTrue(logger.pressedKeys.Contains(new MouseKey(k)));
			}

			Assert.IsEmpty(logger.releasedKeys);

			MouseButton[] b2 = new MouseButton[] { MouseButton.Middle, MouseButton.X1 };

			logger.Clear();
			state = new InputState(mouse: InputUtil.MakeMouseState(buttons: b2));
			manager.Update(state);

			Assert.AreEqual(1, logger.pressedKeys.Count);
			Assert.IsTrue(new MouseKey(MouseButton.X1).Equals(logger.pressedKeys[0]));

			Assert.AreEqual(2, logger.releasedKeys.Count);
			Assert.IsTrue(logger.releasedKeys.Contains(new MouseKey(MouseButton.Left)));
			Assert.IsTrue(logger.releasedKeys.Contains(new MouseKey(MouseButton.X2)));
		}
	}
}
