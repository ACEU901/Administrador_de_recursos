using AdminRecursos;
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

    static async Task DrawGraph(float cpuPercent, float ramPercent, float diskPercent,
        float bandwidthMbps, long latency, float cpuTemperature)
    {
        const int maxValue = 100000;
        const int barLength = 20;

        string cpuBar = new string('#', (int)((cpuPercent / 100) * barLength));
        string ramBar = new string('#', (int)((ramPercent / 100) * barLength));
        string diskBar = new string('#', (int)((diskPercent / 100) * barLength));
        string bandwidthBar = new string('#', (int)((bandwidthMbps / maxValue) * barLength)).PadRight(barLength, '-');
        string latencyBar = new string('#', (int)(latency / 10)).PadRight(barLength, '-');

        cpuBar = cpuBar.PadRight(barLength, '-');
        ramBar = ramBar.PadRight(barLength, '-');
        diskBar = diskBar.PadRight(barLength, '-');

        Console.WriteLine($"CPU: [{cpuBar}] {cpuPercent:F1}%");
        Console.WriteLine($"RAM: [{ramBar}] {ramPercent:F1}%");
        Console.WriteLine($"Disk: [{diskBar}] {diskPercent:F1}%");
        Console.WriteLine($"Bandwidth: [{bandwidthBar}] {bandwidthMbps:F1} Kbps");
        Console.WriteLine($"Latency: [{latencyBar}] {latency} ms");
        Console.WriteLine($"CPU Temperature: {cpuTemperature:F1}°C");

        LocationInfo location = await InfoUbicacion.getLocationAsync();

        if (location != null)
        {
            
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("*************Datos De Ubicación*************");
            Console.WriteLine($"País: {location.country}");
            Console.WriteLine($"Ciudad: {location.city}");
            Console.WriteLine($"Región: {location.region}");
            Console.WriteLine($"IP Pública: {location.ip}");
            Console.WriteLine($"Código Postal: {location.postal}");
        }
        else
        {
            Console.WriteLine("❌ No se pudo obtener la ubicación.");
        }
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
            long latency = MonitorLatencia.ObtenerLatenciaPing("google.com");
            float cpuTemperature = TempInterna.getCpuTemperature();


            DrawGraph(cpu, ram, disk, bandwidthMbps, latency, cpuTemperature);

            if (cpu > 80 || ram > 80 || bandwidthMbps > 8000 || latency > 150)
            {
                Console.WriteLine("CRITICO: Uso excesivo de recursos o alta latencia");
            }
            else if (cpu >= 75 || ram > 75 || bandwidthMbps > 75000 || latency > 100)
            {
                Console.WriteLine("ALERTA: Se debe monitorear recursos y latencia");
            }
            else
            {
                Console.WriteLine("INFO: Salud estable de memoria, procesador y conexión");
            }

            Thread.Sleep(2000);
        }
    }

    static void Main(string[] args)
    {
        MonitorResources();
    }
}
