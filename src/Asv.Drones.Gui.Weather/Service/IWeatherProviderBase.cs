using Asv.Common;

namespace Asv.Drones.Gui.Weather;

public class WeatherData
{
    public double WindSpeed { get; set; } // Property names depend on actual API response
    public double WindDirection { get; set; }
    public double Temperature { get; set; }
}

public interface IWeatherProviderBase
{
    string ApiKey { get; set; }
    string Name { get; }

    Task<WeatherData> GetWeatherData(GeoPoint location);
    Task<WeatherData> GetWeatherData(double latitude, double longitude);
}

