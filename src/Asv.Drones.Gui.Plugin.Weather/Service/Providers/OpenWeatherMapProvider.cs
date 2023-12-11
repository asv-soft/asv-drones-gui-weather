using System.ComponentModel.Composition;
using Asv.Common;
using Asv.Drones.Gui.Core;
using Newtonsoft.Json;

namespace Asv.Drones.Gui.Plugin.Weather;

[Export(typeof(IWeatherProviderBase))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class OpenWeatherMapProvider : IWeatherProviderBase
{
    private readonly ILogService _log;
    public string ApiKey { get; set; }
    public string Name => "OpenWeatherMap";

    [ImportingConstructor]
    public OpenWeatherMapProvider(ILogService log)
    {
        _log = log;
    }
    
    public async Task<WeatherData> GetWeatherData(double latitude, double longitude)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?" +
                            $"lat={latitude}" +
                            $"&lon={longitude}" +
                            $"&appid={ApiKey}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    OpenWeatherMapResponse openWeatherMapResponse =
                        JsonConvert.DeserializeObject<OpenWeatherMapResponse>(jsonResponse);

                    var weatherData = new WeatherData
                    {
                        WindSpeed = openWeatherMapResponse.Wind.Speed,
                        WindDirection = openWeatherMapResponse.Wind.Direction,
                        Temperature = openWeatherMapResponse.Main.Temperature
                    };

                    return weatherData;
                }

                _log.Error(Name,string.Format(RS.WeatherProvider_CustomErrorMessage, response.StatusCode, response.ReasonPhrase));
            }
            catch (Exception ex)
            {
                _log.Error(Name,RS.WeatherProvider_ConnectionErrorMessage, ex);
            }
        }
        return new WeatherData();
    }

    public async Task<WeatherData> GetWeatherData(GeoPoint location)
    {
        return await GetWeatherData(location.Latitude, location.Longitude);
    }
}

public class OpenWeatherMapResponse
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