using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Asv.Cfg;
using Asv.Common;
using Asv.Drones.Gui.Core;

namespace Asv.Drones.Gui.Weather;

public class WeatherServiceConfig
{
    public bool Visibility { get; set; }
    public string CurrentProviderName { get; set; }
    public Dictionary<string, string> ProvidersApiKeys { get; set; } = new();
    
    public WeatherData LastWeatherData { get; set; }
}

[Export(typeof(IWeatherService))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class WeatherService : ServiceWithConfigBase<WeatherServiceConfig>, IWeatherService
{
    private readonly IEnumerable<IWeatherProviderBase> _weatherProviders;

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
    
    public IRxEditableValue<bool> Visibility { get; }

    public IRxEditableValue<WeatherData> LastWeatherData { get; }
    
    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }

    public IRxEditableValue<string> CurrentWeatherProviderApiKey { get; }
    
    public IEnumerable<IWeatherProviderBase> WeatherProviders => _weatherProviders;
    
    private void SetVisibility(bool visibility)
    {
        InternalSaveConfig(_ => _.Visibility = visibility);
    }

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

    private void SetCurrentProviderApiKey(string key)
    {
        CurrentWeatherProvider.Value.ApiKey = key;
        
        InternalSaveConfig(_ => 
            _.ProvidersApiKeys[CurrentWeatherProvider.Value.Name] = key);
    }
    
    private void SetLastWeatherData(WeatherData data)
    {
        InternalSaveConfig(_ =>_.LastWeatherData = data);
    }
    
    public async Task<WeatherData> GetWeatherData(GeoPoint location)
    {
        return await CurrentWeatherProvider.Value.GetWeatherData(location);
    }
}
