using System.ComponentModel.Composition;
using Asv.Common;
using Asv.Drones.Gui.Weather;
using Newtonsoft.Json;

namespace Asv.Drones.Gui.Core;

[Export(typeof(IWeatherProviderBase))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class OpenWeatherMapProvider : IWeatherProviderBase
{
    public string ApiKey { get; set; }
    public string Name => "OpenWeatherMap";
    
    public async Task<WeatherData> GetWeatherData(double latitude, double longitude)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?" +
                            $"lat={latitude}" +
                            $"&lon={longitude}" +
                            $"&appid={ApiKey}";

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            string jsonResponse = await response.Content.ReadAsStringAsync();

            OpenWeatherMapWeatherResponse openWeatherMapWeatherResponse = 
                JsonConvert.DeserializeObject<OpenWeatherMapWeatherResponse>(jsonResponse);
         
            var weatherData = new WeatherData
            {
                WindSpeed = openWeatherMapWeatherResponse.Wind.Speed,
                WindDirection = openWeatherMapWeatherResponse.Wind.Direction,
                Temperature = openWeatherMapWeatherResponse.Main.Temperature - 273.15f // Conversion from Kelvin to Celsius
            };

            return weatherData;
        }
        
        return new WeatherData();
    }

    public async Task<WeatherData> GetWeatherData(GeoPoint location)
    {
        return await GetWeatherData(location.Latitude, location.Longitude);
    }
}

public class OpenWeatherMapWeatherResponse
{
    [JsonProperty("wind")]
    public Wind Wind { get; set; }

    [JsonProperty("main")]
    public Main Main { get; set; }
}

public class Wind 
{
    [JsonProperty("speed")]
    public float Speed { get; set; }

    [JsonProperty("deg")]
    public float Direction { get; set; }
}

public class Main 
{
    [JsonProperty("temp")]
    public float Temperature { get; set; }
}