using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.VisualBasic.FileIO;

namespace AMDFanController
{
    class Program
    {
        private static string CardLocation = "";
        static void Main(string[] args)
        {
            CardLocation = GetGPULocation();
            SetFanMode(FanStates.Manual);
            byte speed = GPUTemp();
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                var Temperature = GPUTemp();
                var DestinationSpeed = SpeedTemp(Temperature);
                Console.WriteLine("Temperature:   " + Temperature);
                Console.WriteLine("Speed (0-255): " + DestinationSpeed);
                SetFanSpeed(DestinationSpeed);
            }
        }

        private static string GetGPULocation()
        {
            return "/sys/class/drm/card0/device/hwmon/hwmon1/";
            //If you can figure out how to generalize this for other GPU's, make it so and
            //Make it a PR!
        }

        static byte SpeedTemp(byte temperature)
        {
            var FanSpeed = (byte) Math.Min(Math.Max(0, temperature - 60) * 255 / 30, 255);
            return FanSpeed; //Failsafe
        }
        static void SetFanSpeed(byte speed)
        {
            string p = speed.ToString();
            File.WriteAllText(CardLocation + "pwm1", p);
        }
        static void SetFanMode(int mode)
        {
            Console.WriteLine("Setting GPU Control");
            string PWM1_Enable = CardLocation + "pwm1_enable";
            File.WriteAllText(PWM1_Enable, mode.ToString());
        }

        static byte GPUTemp()
        {
            string temp = File.ReadAllText(CardLocation + "/temp1_input");
            int temperature = int.Parse(temp) / 1000;
            byte final = Convert.ToByte(temperature);
            return final;
        }

        public static class FanStates
        {
            public const int Maximum = 0;
            public const int Manual = 1;
            public const int Automatic = 2;
        }
    }
}