using Asv.Common;

namespace Asv.Drones.Gui.Weather;

public interface IWeatherService
{
    public IRxEditableValue<bool> Visibility { get; }
    public IEnumerable<IWeatherProviderBase> WeatherProviders { get; }
    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }
}