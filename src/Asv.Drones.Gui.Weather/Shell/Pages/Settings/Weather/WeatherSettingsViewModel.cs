using System.ComponentModel.Composition;
using Asv.Common;
using Asv.Drones.Gui.Core;
using DynamicData.Binding;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Weather;

[Export(typeof(ISettingsPart))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WeatherSettingsViewModel : SettingsPartBase
{
    private static readonly Uri Uri = new(SettingsPartBase.Uri, "weather");
    private readonly IWeatherService _weatherService;

    public WeatherSettingsViewModel() : base(Uri)
    {
        
    }

    [ImportingConstructor]
    public WeatherSettingsViewModel(IWeatherService weatherService) : this()
    {
        _weatherService = weatherService;
        
        _weatherService.Visibility.Subscribe(_ => Visibility = _).DisposeItWith(Disposable);
        
        this.WhenAnyValue(_ => _.Visibility)
            .Subscribe(_weatherService.Visibility)
            .DisposeItWith(Disposable);
        
        this.WhenValueChanged(_ => _.Visibility)
            .Subscribe(_ =>
            {
                VisibilityButton = _
                    ? RS.WeatherSettingsView_WeatherSwitch_Button_Hide
                    : RS.WeatherSettingsView_WeatherSwitch_Button_Show;
            })
            .DisposeItWith(Disposable);
        
        _weatherService.CurrentWeatherProvider.Subscribe(_ => CurrentWeatherProvider = _).DisposeItWith(Disposable);
        
        this.WhenAnyValue(_ => _.CurrentWeatherProvider)
            .Subscribe(_weatherService.CurrentWeatherProvider)
            .DisposeItWith(Disposable);
    }

    public override int Order => 4;
    
    [Reactive]
    public bool Visibility { get; set; }

    [Reactive]
    public string VisibilityButton { get; set; }
    
    [Reactive]
    public IWeatherProviderBase CurrentWeatherProvider { get; set; }

    public IEnumerable<IWeatherProviderBase> WeatherProviders => _weatherService.WeatherProviders;
    public string WeatherIcon => MaterialIconDataProvider.GetData(MaterialIconKind.WeatherPartlyCloudy);
    public string WeatherApiKeyIcon => MaterialIconDataProvider.GetData(MaterialIconKind.Key);
    public string WeatherSwitchIcon => MaterialIconDataProvider.GetData(MaterialIconKind.Visibility);
}