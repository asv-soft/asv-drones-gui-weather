using System.ComponentModel.Composition;
using Asv.Common;
using Asv.Drones.Gui.Core;
using Newtonsoft.Json;

namespace Asv.Drones.Gui.Plugin.Weather;

[Export(typeof(IWeatherProviderBase))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WindyProvider : IWeatherProviderBase
{
    private readonly ILogService _log;
    public string ApiKey { get; set; }
    public string Name => "Windy";
    
    [ImportingConstructor]
    public WindyProvider(ILogService log)
    {
        _log = log;
    }
    
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
                    
                    WindyResponse openWeatherMapResponse = 
                        JsonConvert.DeserializeObject<WindyResponse>(jsonResponse);

                    var windSpeedV = openWeatherMapResponse.WindSpeedV.Average();
                    var windSpeedU = openWeatherMapResponse.WindSpeedU.Average();
                    var temperature = openWeatherMapResponse.Temperature.Average();
                    
                    // Retrieve wind speed, wind angle, and temperature values
                    var weatherData = new WeatherData
                    {
                        WindSpeed =  CalculateResultantSpeed(windSpeedU, windSpeedV),
                        WindDirection = CalculateWindDirection(windSpeedU, windSpeedV),
                        Temperature = temperature
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
    
    private double CalculateWindDirection(double speedU, double speedV)
    {
        // Calculate the wind direction in radians using the Atan2 function
        double windAngleRad = Math.Atan2(speedV, speedU);

        // Convert radians to degrees
        double windAngleDeg = windAngleRad * (180.0 / Math.PI);

        // Adjust the angle to be between 0 and 360 degrees
        if (windAngleDeg < 0)
        {
            windAngleDeg += 360.0;
        }

        return windAngleDeg;
    }
    
    private double CalculateResultantSpeed(double speedU, double speedV)
    {
        // Calculate the resultant speed using the Pythagorean theorem
        double resultantSpeed = Math.Sqrt((speedU * speedU) + (speedV * speedV));

        return resultantSpeed;
    }
}

public class WindyResponse
{
    [JsonProperty("wind_u-surface")]
    public double[] WindSpeedU { get; set; }

    [JsonProperty("wind_v-surface")]
    public double[] WindSpeedV { get; set; }
    
    [JsonProperty("temp-surface")]
    public double[] Temperature { get; set; }
}



