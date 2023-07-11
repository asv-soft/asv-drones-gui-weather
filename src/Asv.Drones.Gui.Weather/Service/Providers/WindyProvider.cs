using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Drones.Gui.Weather;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Asv.Drones.Gui.Core;

[Export(typeof(IWeatherProviderBase))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WindyProvider : IWeatherProviderBase
{
    public string ApiKey { get; set; }
    public string Name => "Windy";
    
    public async Task<WeatherData> GetWeatherData(double latitude, double longitude)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiUrl = "https://api.windy.com/api/point-forecast/v2";

            // Create the content to send in the request body
            var requestData = new
            {
                lat = latitude,
                lon = longitude,
                model = "gfs",
                parameters = new[] {"wind", "temp"}, 
                key = ApiKey
            };
            
            // Serialize the request data to JSON
            string jsonRequest = JsonConvert.SerializeObject(requestData);

            // Create the HttpContent object from the JSON string
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject forecastData = JObject.Parse(jsonResponse);
                    
                    // // Extract wind and temperature data
                    // JArray windArray = forecastData["forecast"]["gfs"]["surface"]["wind"]["values"] as JArray;
                    // JArray temperatureArray = forecastData["forecast"]["gfs"]["surface"]["temperature"]["values"] as JArray;
                    //
                    // // Access the latest forecast data
                    // JObject latestWind = windArray.Last as JObject;
                    // JObject latestTemperature = temperatureArray.Last as JObject;
                    //
                    // // Retrieve wind speed, wind angle, and temperature values
                    // var weatherData = new WeatherData
                    // {
                    //     WindSpeed = (double)latestWind["ws"],
                    //     WindDirection = (double)latestWind["wd"],
                    //     Temperature = (double)latestTemperature["value"]
                    // };
                    //
                    // Console.WriteLine($"Wind Speed: {windSpeed} m/s");
                    // Console.WriteLine($"Wind Angle: {windAngle} degrees");
                    // Console.WriteLine($"Temperature: {temperature} °C");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
            // try
            // {
            //     HttpResponseMessage response = await client.GetAsync(apiUrl);
            //
            //     // Create the content to send in the request body
            //     var requestData = new { username = "john", password = "password123" };
            //
            //     // Serialize the request data to JSON
            //     string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            //
            //     // Create the HttpContent object from the JSON string
            //     var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            //
            //     try
            //     {
            //         HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            //
            //         if (response.IsSuccessStatusCode)
            //         {
            //             string jsonResponse = await response.Content.ReadAsStringAsync();
            //             // Process the response JSON as needed
            //             Console.WriteLine(jsonResponse);
            //         }
            //         else
            //         {
            //             Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"An error occurred: {ex.Message}");
            //     }
            //     
            //     if (response.IsSuccessStatusCode)
            //     {
            //         string jsonResponse = await response.Content.ReadAsStringAsync();
            //         JObject forecastData = JObject.Parse(jsonResponse);
            //
            //         // Extract wind and temperature data
            //         JArray windArray = forecastData["forecast"]["ecmwf"]["surface"]["wind"]["values"] as JArray;
            //         JArray temperatureArray = forecastData["forecast"]["ecmwf"]["surface"]["temperature"]["values"] as JArray;
            //
            //         // Access the latest forecast data
            //         JObject latestWind = windArray.Last as JObject;
            //         JObject latestTemperature = temperatureArray.Last as JObject;
            //
            //         // Retrieve wind speed, wind angle, and temperature values
            //         var weatherData = new WeatherData
            //         {
            //             WindSpeed = (double)latestWind["ws"],
            //             WindDirection = (double)latestWind["wd"],
            //             Temperature = (double)latestTemperature["value"]
            //         };
            //         
            //         Console.WriteLine($"Wind Speed: {windSpeed} m/s");
            //         Console.WriteLine($"Wind Angle: {windAngle} degrees");
            //         Console.WriteLine($"Temperature: {temperature} °C");
            //     }
            //     else
            //     {
            //         Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"An error occurred: {ex.Message}");
            // }
        }
        
        return new WeatherData();
    }

    public async Task<WeatherData> GetWeatherData(GeoPoint location)
    {
        return await GetWeatherData(location.Latitude, location.Longitude);
    }
}




