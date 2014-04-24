using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{
    public enum EventCode
    {
        None = 0,
        Left,
        Right,
        Jump,
    }

    public class InputEvent
    {
        public InputEvent(EventCode code, ButtonState state)
        {
            Code = code;
            State = state;
        }

        public EventCode Code;
        public ButtonState State;
    }

    public interface InputEventGenerator
    {
        InputEvent Update(InputState state);
    }

    public class HoldEventGenerator
    {
        public RawKey ListenKey
        {
            get;
            private set;
        }

        public ButtonState State
        {
            get;
            private set;
        }
        public EventCode Code
        { 
            get; 
            private set; 
        }

        public HoldEventGenerator(RawKey key, EventCode code)
        {
            ListenKey = key;
            State = ButtonState.Released;
            Code = code;
        }

        public InputEvent Update(InputState state)
        {
            if (state.GetButtonState(ListenKey) != State)
            {
                if (State == ButtonState.Pressed)
                {
                    State = ButtonState.Released;
                    return new InputEvent(Code, State);
                }
                else
                {
                    State = ButtonState.Pressed;
                    return new InputEvent(Code, State);
                }
            }

            return new InputEvent(EventCode.None, ButtonState.Released);
        }

        public void SetKey(RawKey key)
        {
            ListenKey = key;
        }
    }
}
