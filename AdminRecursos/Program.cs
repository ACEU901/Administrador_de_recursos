using System;
using System.Diagnostics;
using System.Threading;

class ResourceMonitor
{
    static PerformanceCounter cpuCounter = new PerformanceCounter(
        "Processor", "% Processor Time", "_Total");

    static PerformanceCounter ramCounter = new PerformanceCounter(
        "Memory", "% Committed Bytes In Use");
    static PerformanceCounter diskCounter = new PerformanceCounter(
       "PhysicalDisk", "% Disk Time", "_Total");
    static PerformanceCounter bandwidthSentCounter = new PerformanceCounter(
        "Network Interface", "Bytes Sent/sec", GetNetworkInterface());

    static PerformanceCounter bandwidthReceivedCounter = new PerformanceCounter(
        "Network Interface", "Bytes Received/sec", GetNetworkInterface());
    //TODO: Temperatura se CPU,  Temperatura Ambiente
    static string GetNetworkInterface()
    {
        foreach (var category in new PerformanceCounterCategory("Network Interface").GetInstanceNames())
        {
            if (!category.Contains("_Total"))
                return category;
        }
        return "";
    }

    static void ClearScreen()
    {
        Console.Clear();
    }

    static void DrawGraph(float cpuPercent, float ramPercent, float diskPercent, float bandwidthMbps)
    {
        const int maxValue = 100000;
        const int barLength = 20;

        string cpuBar = new string('#', (int)((cpuPercent / 100) * barLength));
        string ramBar = new string('#', (int)((ramPercent / 100) * barLength));
        string diskBar = new string('#', (int)((diskPercent / 100) * barLength));
        string bandwidthBar = new string('#', (int)((bandwidthMbps / maxValue) * barLength)).PadRight(barLength, '-');

        cpuBar = cpuBar.PadRight(barLength, '-');
        ramBar = ramBar.PadRight(barLength, '-');
        diskBar = diskBar.PadRight(barLength, '-');

        Console.WriteLine($"CPU: [{cpuBar}] {cpuPercent:F1}%");
        Console.WriteLine($"RAM: [{ramBar}] {ramPercent:F1}%");
        Console.WriteLine($"Disk: [{diskBar}] {diskPercent:F1}%");
        Console.WriteLine($"Bandwidth: [{bandwidthBar}] {bandwidthMbps:F1} Kbps");

    }

    static void MonitorResources()
    {
        while (true)
        {
            ClearScreen();
            float cpu = cpuCounter.NextValue();
            float ram = ramCounter.NextValue();
            float disk = diskCounter.NextValue();

            float bandwidthSent = bandwidthSentCounter.NextValue();
            float bandwidthReceived = bandwidthReceivedCounter.NextValue();
            float bandwidthMbps = (bandwidthSent + bandwidthReceived) * 8 / 1000;


            DrawGraph(cpu, ram, disk, bandwidthMbps);

            if (cpu > 80 || ram > 80 || bandwidthMbps > 8000)
            {
                Console.WriteLine("ALERTA: Uso excesivo de recursos");
            }
            else if (cpu >= 75 || ram > 75 || bandwidthMbps > 75000 )
            {
                Console.WriteLine("ALERTA: Se debe monitorear recursos");
            }
            else
            {
                Console.WriteLine("ALERTA: Salud estable de memoria ram y procesador");
            }

            Thread.Sleep(2000);
        }
    }

    static void Main(string[] args)
    {
        MonitorResources();
    }
}
