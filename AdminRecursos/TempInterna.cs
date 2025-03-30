using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace AdminRecursos
{
    class TempInterna
    {

        public static float getCpuTemperature()
        {
            float temperature = 0;
            Computer computer = new Computer
            {
                IsCpuEnabled = true
            };
            computer.Open();

            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    hardware.Update();
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            temperature = sensor.Value.GetValueOrDefault();
                            break;
                        }
                    }
                }
            }
            computer.Close();
            return temperature;
        }
    }
}
