using Newtonsoft.Json.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Application.Utils
{
    public static class GetDistanceMatrix
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _apiKey;
        public static void Initialize(string apikey)
        {
            _apiKey = apikey;
        }

        public static async Task<float> GetDistanceAsync(string origin, string destination)
        {
            string url = $"https://api.distancematrix.ai/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);

                var distanceText = data["rows"][0]["elements"][0]["distance"]["text"].ToString();

                var distanceValue = int.Parse(System.Text.RegularExpressions.Regex.Match(distanceText, @"\d+").Value);

                var duration = data["rows"][0]["elements"][0]["duration"]["text"];

                Console.WriteLine($"Distance: {distanceText}");
                Console.WriteLine($"Duration: {duration}");
                return distanceValue;

            }
            else
            {
                Console.WriteLine("Error: Could not retrieve data from Distance Matrix API.");
                return -1;
            }
        }
    }
}
