using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Drones.Gui.Plugin.Weather;

public class WeatherData
{
    public double WindSpeed { get; set; }
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

