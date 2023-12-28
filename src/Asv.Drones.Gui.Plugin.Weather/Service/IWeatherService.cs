using System.Collections.Generic;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Drones.Gui.Plugin.Weather;

/// Represents a weather service that retrieves weather data from multiple weather providers.
/// /
public interface IWeatherService
{
    public IRxEditableValue<bool> Visibility { get; }

    /// <summary>
    /// Represents a collection of weather providers. </summary>
    /// <value>
    /// An IEnumerable of IWeatherProviderBase objects representing the weather providers. </value>
    /// /
    public IEnumerable<IWeatherProviderBase> WeatherProviders { get; }

    /// Gets the API key for the current weather provider.
    /// The API key is required to access the current weather information from the provider.
    /// @return The API key as an IRxEditableValue of string.
    /// /
    public IRxEditableValue<string> CurrentWeatherProviderApiKey { get; }

    /// <summary>
    /// Gets the current weather provider.
    /// </summary>
    /// <remarks>
    /// This property retrieves the current weather provider used to fetch weather data. The weather provider must implement the
    /// IWeatherProviderBase interface.
    /// </remarks>
    /// <returns>
    /// An object that implements the IRxEditableValue interface with the type IWeatherProviderBase.
    /// </returns>
    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }

    /// <summary>
    /// Gets the last weather data.
    /// </summary>
    /// <value>
    /// The last weather data.
    /// </value>
    public IRxEditableValue<WeatherData> LastWeatherData { get; }

    /// <summary>
    /// Retrieves weather data for a given location.
    /// </summary>
    /// <param name="location">The geographical coordinates of the location.</param>
    /// <returns>A task representing the asynchronous operation that returns the weather data for the specified location.</returns>
    public Task<WeatherData> GetWeatherData(GeoPoint location);
}