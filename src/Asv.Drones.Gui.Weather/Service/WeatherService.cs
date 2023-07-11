using System.ComponentModel.Composition;
using Asv.Cfg;
using Asv.Common;
using Asv.Drones.Gui.Core;

namespace Asv.Drones.Gui.Weather;

public class WeatherServiceConfig
{
    public bool Visibility { get; set; } = false;
    public string CurrentProviderName { get; set; }
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

        CurrentWeatherProvider = new RxValue<IWeatherProviderBase>(
            weatherProviders.SingleOrDefault(_ => _.Name == weatherProviderFromConfig))
            .DisposeItWith(Disposable);

        CurrentWeatherProvider.Subscribe(SetCurrentProvider).DisposeItWith(Disposable);
        
        _weatherProviders = weatherProviders;
    }
    
    public IRxEditableValue<bool> Visibility { get; }

    public IRxEditableValue<IWeatherProviderBase> CurrentWeatherProvider { get; }

    public IEnumerable<IWeatherProviderBase> WeatherProviders => _weatherProviders;
    
    private void SetVisibility(bool visibility)
    {
        InternalSaveConfig(_ => _.Visibility = visibility);
    }

    private void SetCurrentProvider(IWeatherProviderBase provider)
    {
        //CurrentWeatherProvider = WeatherProviders.SingleOrDefault(_ => _.Name == provider.Name);
        InternalSaveConfig(_ => _.CurrentProviderName = provider.Name);
    }
}
