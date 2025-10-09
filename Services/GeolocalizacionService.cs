using System.Text.Json;
using System.Net.Http.Json;
using PWAs.Models.Geolocalizacion;

namespace PWAs.Services
{
    public class GeolocalizacionService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://nominatim.openstreetmap.org/reverse";

        public GeolocalizacionService()
        {
        }

        public async Task<string> GetAddressFromCoordinates(double latitude, double longitude)
        {
            string url = $"{BaseUrl}?lat={latitude}&lon={longitude}&format=json&zoom=18";

            try
            {
                var result = await _httpClient.GetFromJsonAsync<NominatimResult>(url);

                return result?.Display_Name ?? "Direccion no encontrada";
            }
            catch
            {
                return $"Error al obtener los resultados";
            }
        }

        private async Task<string> GetServerPublicIp()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://api.ipify.org");
                return response.Trim();
            }
        }

        public async Task<Localizacion> ObtenerUbicacionServidorLocal()
        {
            var publicIp = await GetServerPublicIp();

            using (var client = new HttpClient())
            {
                string url = $"http://ip-api.com/json/{publicIp}?fields=lat,lon";

                var response = await client.GetStringAsync(url);
                var geoData = JsonSerializer.Deserialize<IpGeolocation>(response);

                if (geoData?.lat.HasValue == true && geoData?.lon.HasValue == true)
                {
                    return new Localizacion
                    {
                        Latitud = geoData.lat.Value,
                        Longitud = geoData.lon.Value
                    };
                }

                return new Localizacion { Latitud = 0, Longitud = 0 };
            }
        }

        public class NominatimResult
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
            public string Display_Name { get; set; }
        }
    }
}
