using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework.Input;
using gravitymania.input;
using Microsoft.Xna.Framework;

namespace gravitymaniaTest.input
{
    [TestFixture]
    class TestRawKey
    {
       [Test]
       public void TestStringMapOnKeyboardKeys()
       {
           RawKey keyboardA = RawKey.Find("A");

           Assert.AreEqual(typeof(KeyboardKey), keyboardA.GetType());
           Assert.AreEqual(Keys.A, (keyboardA as KeyboardKey).Key);

           // case-insensitive search
           RawKey keyboarda = RawKey.Find("a");

           Assert.AreEqual(typeof(KeyboardKey), keyboarda.GetType());
           Assert.AreEqual(Keys.A, (keyboarda as KeyboardKey).Key);
       }

       [Test]
       public void TestStringMapOnKeyboardKeysUsingRawInputChars()
       {
           RawKey keyboardBackSlash = RawKey.Find("\\");

           Assert.AreEqual(typeof(KeyboardKey), keyboardBackSlash.GetType());
           Assert.AreEqual(Keys.OemPipe, (keyboardBackSlash as KeyboardKey).Key);

           RawKey keyboardQuestion = RawKey.Find("?");

           Assert.AreEqual(typeof(KeyboardKey), keyboardQuestion.GetType());
           Assert.AreEqual(Keys.OemQuestion, (keyboardQuestion as KeyboardKey).Key);

           RawKey keyboardEq = RawKey.Find("=");

           Assert.AreEqual(typeof(KeyboardKey), keyboardEq.GetType());
           Assert.AreEqual(Keys.OemPlus, (keyboardEq as KeyboardKey).Key);
       }

       [Test]
       public void TestStringMapOnKeyboardKeysUsingRawInputCharsAmbiguity()
       {
           RawKey keyboardPlus = RawKey.Find("+");

           Assert.AreEqual(typeof(KeyboardKey), keyboardPlus.GetType());
           Assert.AreEqual(Keys.OemPlus, (keyboardPlus as KeyboardKey).Key);

           RawKey keyboardMinus = RawKey.Find("-");

           Assert.AreEqual(typeof(KeyboardKey), keyboardMinus.GetType());
           Assert.AreEqual(Keys.OemMinus, (keyboardMinus as KeyboardKey).Key);

           RawKey keyboardStar = RawKey.Find("*");

           Assert.AreEqual(typeof(KeyboardKey), keyboardStar.GetType());
           Assert.AreEqual(Keys.D8, (keyboardStar as KeyboardKey).Key);

           RawKey keyboardFive = RawKey.Find("5");

           Assert.AreEqual(typeof(KeyboardKey), keyboardFive.GetType());
           Assert.AreEqual(Keys.D5, (keyboardFive as KeyboardKey).Key);
       }

       [Test]
       public void TestStringMapOnX360Keys()
       {
           RawKey gamepad0A = RawKey.Find("XPad0.A");

           Assert.AreEqual(typeof(X360PadKey), gamepad0A.GetType());
           Assert.AreEqual(PlayerIndex.One, (gamepad0A as X360PadKey).Controller);
           Assert.AreEqual(Buttons.A, (gamepad0A as X360PadKey).Button);

           // case-insensitive search
           RawKey gamepad3start = RawKey.Find("xpad3.sTart");

           Assert.AreEqual(typeof(X360PadKey), gamepad3start.GetType());
           Assert.AreEqual(PlayerIndex.Four, (gamepad3start as X360PadKey).Controller);
		   Assert.AreEqual(Buttons.Start, (gamepad3start as X360PadKey).Button);
       }

       [Test]
       public void TestStringMapOnJoypadKeys()
       {
           RawKey gamepad0B2 = RawKey.Find("Pad6.B2");

           Assert.AreEqual(typeof(JoypadDigitalKey), gamepad0B2.GetType());
           Assert.AreEqual(6, (gamepad0B2 as JoypadDigitalKey).ControllerIndex);
           Assert.AreEqual(2, (gamepad0B2 as JoypadDigitalKey).ButtonIndex);

           // case-insensitive search
           RawKey gamepad0B31 = RawKey.Find("pAd0.B31");

           Assert.AreEqual(typeof(JoypadDigitalKey), gamepad0B31.GetType());
           Assert.AreEqual(0, (gamepad0B31 as JoypadDigitalKey).ControllerIndex);
           Assert.AreEqual(31, (gamepad0B31 as JoypadDigitalKey).ButtonIndex);
       }

       [Test]
       public void TestStringMapOnJoypadAnalogs()
       {
           RawKey gamepad0B2 = RawKey.Find("Pad9.X+");

           Assert.AreEqual(typeof(JoypadAnalogKey), gamepad0B2.GetType());
           Assert.AreEqual(9, (gamepad0B2 as JoypadAnalogKey).ControllerIndex);
           Assert.AreEqual(JoypadAnalogAxis.X, (gamepad0B2 as JoypadAnalogKey).Axis);
           Assert.AreEqual(JoypadAnalogDirection.Positive, (gamepad0B2 as JoypadAnalogKey).Direction);

           // case-insensitive search
           RawKey gamepad0B31 = RawKey.Find("pAd0.U-");

           Assert.AreEqual(typeof(JoypadAnalogKey), gamepad0B31.GetType());
           Assert.AreEqual(JoypadAnalogAxis.U, (gamepad0B31 as JoypadAnalogKey).Axis);
           Assert.AreEqual(JoypadAnalogDirection.Negative, (gamepad0B31 as JoypadAnalogKey).Direction);
       }
    }
}
