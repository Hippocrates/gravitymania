using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace gravitymania.input
{

    public interface EventData
    {
    }

    public class HoldEventData : EventData
    {
        public HoldEventData(ButtonState state)
        {
            State = state;
        }

        public ButtonState State;
    }

    public interface InputEventGenerator
    {
        EventData Update(InputState state);
    }

    public class HoldEventGenerator : InputEventGenerator
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

        public HoldEventGenerator(RawKey key)
        {
            ListenKey = key;
            State = ButtonState.Released;
        }

        public EventData Update(InputState state)
        {
            if (state.GetButtonState(ListenKey) != State)
            {
                if (State == ButtonState.Pressed)
                {
                    State = ButtonState.Released;
                    return new HoldEventData(State);
                }
                else
                {
                    State = ButtonState.Pressed;
                    return new HoldEventData(State);
                }
            }

            return null;
        }

        public void SetListenKey(RawKey key)
        {
            ListenKey = key;
        }
    }
}
