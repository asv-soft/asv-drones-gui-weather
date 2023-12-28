using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Asv.Cfg;
using Asv.Common;
using Asv.Drones.Gui.Core;

namespace Asv.Drones.Gui.Plugin.Weather;

/// <summary>
/// This class represents the configuration settings for the weather service.
/// </summary>
public class WeatherServiceConfig
{
    /// <summary>
    /// Gets or sets the visibility of the object.
    /// </summary>
    /// <value>
    /// True if the object is visible; otherwise, false.
    /// </value>
    public bool Visibility { get; set; }

    /// <summary>
    /// Gets or sets the name of the current provider.
    /// </summary>
    /// <value>
    /// The name of the current provider.
    /// </value>
    public string CurrentProviderName { get; set; }

    /// <summary>
    /// Dictionary property that stores the API keys for providers.
    /// </summary>
    /// <remarks>
    /// The API keys are stored as key-value pairs, where the key is a string representing the provider name,
    /// and the value is a string representing the API key for that provider.
    /// </remarks>
    /// <value>
    /// A dictionary containing the API keys for providers.
    /// </value>
    public Dictionary<string, string> ProvidersApiKeys { get; set; } = new();

    /// <summary>
    /// Gets or sets the last weather data.
    /// </summary>
    /// <value>
    /// The last weather data.
    /// </value>
    public WeatherData LastWeatherData { get; set; }
}

/// <summary>
/// Represents a weather service that provides weather data.
/// </summary>
[Export(typeof(IWeatherService))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class WeatherService : ServiceWithConfigBase<WeatherServiceConfig>, IWeatherService
{
    /// <summary>
    /// Represents a collection of weather providers.
    /// </summary>
    private readonly IEnumerable<IWeatherProviderBase> _weatherProviders;

    /// Creates a new instance of the WeatherService class.
    /// @param cfg The configuration object used to retrieve settings.
    /// @param weatherProviders The collection of weather providers to choose from.
    /// @throws ArgumentNullException if cfg is null.
    /// /
    [ImportingConstructor]
    public WeatherService(IConfiguration cfg, [ImportMany] IEnumerable<IWeatherProviderBase> weatherProviders) : base(cfg)
    {
        if (cfg == null) throw new ArgumentNullException(nameof(cfg));
        
        var visibilityFromConfig = InternalGetConfig(_ => _.Visibility);
        
        Visibility = new RxValue<bool>(visibilityFromConfig).DisposeItWith(Disposable);
        
        Visibility.Subscribe(SetVisibility).DisposeItWith(Disposable);
        
        var weatherProviderFromConfig = InternalGetConfig(_ => _.CurrentProviderName);

        if (weatherProviderFromConfig.IsNullOrWhiteSpace())
        {
            CurrentWeatherProvider = new RxValue<IWeatherProviderBase>(
                    weatherProviders.FirstOrDefault())
                .DisposeItWith(Disposable);
        }
        else
        {
            CurrentWeatherProvider = new RxValue<IWeatherProviderBase>(
                    weatherProviders.SingleOrDefault(_ => _.Name == weatherProviderFromConfig))
                .DisposeItWith(Disposable);
        }
        
        CurrentWeatherProvider.Subscribe(SetCurrentProvider).DisposeItWith(Disposable);
        
        var weatherProviderApiKeyFromConfig = 
            InternalGetConfig(_ =>
            {
                if (_.ProvidersApiKeys == null) _.ProvidersApiKeys = new();
                if (_.ProvidersApiKeys.TryGetValue(CurrentWeatherProvider.Value.Name, out var __))
                {
                    return __;
                }
                return "";
            });

        CurrentWeatherProviderApiKey = new RxValue<string>(weatherProviderApiKeyFromConfig)
            .DisposeItWith(Disposable);
        
        CurrentWeatherProviderApiKey.Subscribe(SetCurrentProviderApiKey).DisposeItWith(Disposable);
        
        var lastWeatherDataFromConfig = InternalGetConfig(_ => _.LastWeatherData);
        
        LastWeatherData = new RxValue<WeatherData>(lastWeatherDataFromConfig ?? new WeatherData()).DisposeItWith(Disposable);
        
        LastWeatherData.Subscribe(SetLastWeatherData).DisposeItWith(Disposable);
        
        _weatherProviders = weatherProviders;
    }

    /// <summary>
    /// Gets the visibility of the property.
    /// </summary>
    /// <value>
    /// An <see cref="IRxEditableValue{T}"/> representing the visibility of the property.
    /// </value>
    public IRxEditableValue<bool> Visibility { get; }

    /// <summary>
    /// Gets or sets the last weather data.
    /// </summary>
    /// <value>
    /// The last weather data.
    /// </value>
    public IRxEditableValue<WeatherData> LastWeatherData { get; }

    /// <summary>
    /// Gets or sets the current weather provider.
    /// </summary>
    /// <remarks>
    /// The CurrentWeatherProvider property represents the current weather provider used for retrieving weather information.
    /// </remarks>
    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }

    /// <summary>
    /// Gets the API key used by the current weather provider.
    /// </summary>
    /// <value>
    /// The API key used for accessing the weather provider's API.
    /// </value>
    public IRxEditableValue<string> CurrentWeatherProviderApiKey { get; }

    /// <summary>
    /// Gets the collection of weather providers.
    /// </summary>
    /// <returns>An enumerable of <see cref="IWeatherProviderBase"/> objects.</returns>
    public IEnumerable<IWeatherProviderBase> WeatherProviders => _weatherProviders;

    /// <summary>
    /// Sets the visibility of an object.
    /// </summary>
    /// <param name="visibility">A boolean value indicating the visibility state.</param>
    private void SetVisibility(bool visibility)
    {
        InternalSaveConfig(_ => _.Visibility = visibility);
    }

    /// <summary>
    /// Set the current weather provider.
    /// </summary>
    /// <param name="provider">The weather provider to set as the current provider.</param>
    private async void SetCurrentProvider(IWeatherProviderBase provider)
    {
        if (CurrentWeatherProviderApiKey != null)
        {
            var apiKey = InternalGetConfig(_ =>
            {
                if (_.ProvidersApiKeys == null) 
                    _.ProvidersApiKeys = new();
                
                if (_.ProvidersApiKeys.TryGetValue(provider.Name, out var __))
                {
                    return __;
                }
                return "";
            });
            
            CurrentWeatherProviderApiKey.OnNext(apiKey);

            CurrentWeatherProvider.Value.ApiKey = apiKey;
        }
        
        InternalSaveConfig(_ => _.CurrentProviderName = provider.Name);
    }

    /// <summary>
    /// Sets the API key for the current weather provider.
    /// </summary>
    /// <param name="key">The API key to be set.</param>
    private void SetCurrentProviderApiKey(string key)
    {
        CurrentWeatherProvider.Value.ApiKey = key;
        
        InternalSaveConfig(_ => 
            _.ProvidersApiKeys[CurrentWeatherProvider.Value.Name] = key);
    }

    /// <summary>
    /// Sets the last weather data.
    /// </summary>
    /// <param name="data">The weather data to be set.</param>
    private void SetLastWeatherData(WeatherData data)
    {
        InternalSaveConfig(_ =>_.LastWeatherData = data);
    }

    /// <summary>
    /// Retrieves the weather data for the specified location.
    /// </summary>
    /// <param name="location">The geographical coordinates of the location.</param>
    /// <returns>A Task object representing the asynchronous operation. The result of the operation is the WeatherData object containing the weather information.</returns>
    public async Task<WeatherData> GetWeatherData(GeoPoint location)
    {
        return await CurrentWeatherProvider.Value.GetWeatherData(location);
    }
}
