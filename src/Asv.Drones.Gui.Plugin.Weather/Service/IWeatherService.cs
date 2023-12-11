﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Drones.Gui.Plugin.Weather;

public interface IWeatherService
{
    public IRxEditableValue<bool> Visibility { get; }
    public IEnumerable<IWeatherProviderBase> WeatherProviders { get; }
    public IRxEditableValue<string> CurrentWeatherProviderApiKey { get; }
    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }

    public IRxEditableValue<WeatherData> LastWeatherData { get; }
    public Task<WeatherData> GetWeatherData(GeoPoint location);
}