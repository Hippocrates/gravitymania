using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;

namespace BurgerCube.input
{
    public static class DX8Joypad
    {
        private static List<Device> Joypads = new List<Device>();

        public const int JoypadAxisRange = 5000;

        static DX8Joypad()
        {
            EnumerateDevices();
        }

        public static void EnumerateDevices()
        {
            //create joystick device.
            foreach (DeviceInstance di in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
            {
                Joypads.Add(new Device(di.InstanceGuid));
            }

            foreach (Device joypad in Joypads)
            {
                //Set joystick axis ranges.
                foreach (DeviceObjectInstance doi in joypad.Objects)
                {
                    if ((doi.ObjectId & (int)DeviceObjectTypeFlags.Axis) != 0)
                    {
                        joypad.Properties.SetRange(
                            ParameterHow.ById,
                            doi.ObjectId,
                            new InputRange(-JoypadAxisRange, JoypadAxisRange));
                    }
                }

                //Set joystick axis mode absolute.
                joypad.Properties.AxisModeAbsolute = true;
            }
        }

        public static int NumJoypads()
        {
            return Joypads.Count;
        }

        public static JoystickState GetState(int controllerIndex)
        {
            return Joypads[controllerIndex].CurrentJoystickState;
        }

        public static JoystickState[] GetStates()
        {
            return Joypads.Select(j => j.CurrentJoystickState).ToArray();
        }
    }
}
