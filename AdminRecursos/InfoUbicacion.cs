using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminRecursos
{
    class InfoUbicacion
    {
        public static async Task<LocationInfo> getLocationAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("https://ipinfo.io/json");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<LocationInfo>(responseBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo la ubicación: {ex.Message}");
                return null;
            }
        }
    }
}
