using System.Collections.Generic;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Drones.Gui.Weather;

public interface IWeatherService
{
    public IRxEditableValue<bool> Visibility { get; }
    public IEnumerable<IWeatherProviderBase> WeatherProviders { get; }
    public IRxEditableValue<string> CurrentWeatherProviderApiKey { get; }
    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }

    public Task<WeatherData> GetWeatherData(GeoPoint location);
}