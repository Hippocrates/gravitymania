using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gravitymania.input
{
    public interface InputEventListener
    {
        void InputEvent(EventKey e, EventData d);
    }

    public class InputEventManager
    {
        private Dictionary<EventKey, InputEventListener> Listeners = new Dictionary<EventKey, InputEventListener>();
        private Dictionary<EventKey, InputEventGenerator> Generators = new Dictionary<EventKey, InputEventGenerator>();

        public InputEventManager()
        {
        }

        public void RunInput(InputState input)
        {
            foreach (var kv in Generators)
            {
                if (Listeners.ContainsKey(kv.Key))
                {
                    EventData data = kv.Value.Update(input);

                    if (data != null)
                    {
                        Listeners[kv.Key].InputEvent(kv.Key, data);
                    }
                }
            }
        }

        public void SetInputGenerator(EventKey code, InputEventGenerator generator)
        {
            Generators[code] = generator;
        }

        public void ClearInputGenerator(EventKey code)
        {
            if (Generators.ContainsKey(code))
            {
                Generators.Remove(code);
            }
        }

        public void SetInputEventListener(EventKey code, InputEventListener listener)
        {
            Listeners[code] = listener;
        }

        public void ClearInputEventListener(EventKey code)
        {
            if (Listeners.ContainsKey(code))
            {
                Listeners.Remove(code);
            }
        }

        public void RemoveInputEventListener(InputEventListener listener)
        {
            foreach (var kv in Listeners.ToArray())
            {
                if (kv.Value == listener)
                {
                    Listeners.Remove(kv.Key);
                }
            }
        }
    }
}
