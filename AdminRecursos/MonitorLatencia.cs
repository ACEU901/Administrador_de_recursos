using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.Generic;

class MonitorLatencia
{
    public static void IniciarMonitoreo()
    {
        string host = "google.com";
        Console.WriteLine($"Monitoreando latencia de {host}...");
        Console.WriteLine("Presiona Ctrl+C para detener.");

        List<int> latencias = new List<int>();
        int conteoPing = 1;
        
        while (true)
        {
            long latencia = ObtenerLatenciaPing(host);

            if (latencia == -1)
            {
                Console.WriteLine("Error en la medición de latencia.");
                continue;
            }

            latencias.Add((int)latencia);
            Console.Clear();
            Console.WriteLine($"Monitoreando latencia de {host}...");
            Console.WriteLine("Presiona Ctrl+C para detener.");
            Console.WriteLine("Gráfico de latencia (cada barra representa 1ms de latencia):");

            foreach (int lat in latencias)
            {
                int longitudBarra = lat;
                if (lat < 30) Console.ForegroundColor = ConsoleColor.Green;
                else if (lat < 80) Console.ForegroundColor = ConsoleColor.Yellow;
                else if (lat < 150) Console.ForegroundColor = ConsoleColor.DarkYellow;
                else Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine($"Ping #{latencias.IndexOf(lat) + 1}: {new string('#', longitudBarra)} ({lat} ms)");
                Console.ResetColor();
            }

            Console.WriteLine($"\nÚltimo ping: Ping #{conteoPing} a {host} - Latencia: {latencia} ms - Hora: {DateTime.Now:HH:mm:ss}");
            Thread.Sleep(1000);
            conteoPing++;
        }
    }

    public static long ObtenerLatenciaPing(string host)
    {
        try
        {
            using (var ping = new Ping())
            {
                PingReply respuesta = ping.Send(host);
                return respuesta.Status == IPStatus.Success ? respuesta.RoundtripTime : -1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error de ping: {ex.Message}");
            return -1;
        }
    }
}
