using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Drones.Gui.Plugin.Weather;

/// <summary>
/// Represents weather data including wind speed, wind direction, and temperature.
/// </summary>
public class WeatherData
{
    /// <summary>
    /// Gets or sets the wind speed.
    /// </summary>
    /// <value>
    /// The wind speed.
    /// </value>
    public double WindSpeed { get; set; }

    /// <summary>
    /// Gets or sets the direction of the wind in degrees.
    /// </summary>
    /// <value>
    /// The direction of the wind in degrees.
    /// </value>
    public double WindDirection { get; set; }

    /// <summary>
    /// Gets or sets the temperature.
    /// </summary>
    /// <value>
    /// The temperature value.
    /// </value>
    public double Temperature { get; set; }
}

/// <summary>
/// Represents a weather provider interface.
/// </summary>
public interface IWeatherProviderBase
{
    /// <summary>
    /// Get or set the ApiKey used for authentication.
    /// </summary>
    /// <value>
    /// The ApiKey value.
    /// </value>
    string ApiKey { get; set; }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <returns>The name of the property.</returns>
    string Name { get; }

    /// <summary>
    /// Retrieves the weather data for a given location.
    /// </summary>
    /// <param name="location">The geographical coordinates of the location for which weather data is to be retrieved.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation. The task result contains the WeatherData object that represents the weather data for the specified location.
    /// </returns>
    Task<WeatherData> GetWeatherData(GeoPoint location);

    /// <summary>
    /// Retrieves the weather data for the given latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude of the location.</param>
    /// <param name="longitude">The longitude of the location.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the weather data for the given location.</returns>
    Task<WeatherData> GetWeatherData(double latitude, double longitude);
}

