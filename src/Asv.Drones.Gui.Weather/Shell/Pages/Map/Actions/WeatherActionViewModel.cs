using System.ComponentModel.Composition;
using System.Windows.Input;
using Asv.Common;
using Asv.Drones.Gui.Core;
using Avalonia.Controls;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Weather;

[Export(FlightPageViewModel.UriString,typeof(IMapAction))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WeatherActionViewModel : MapActionBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILocalizationService _localizationService;

    public WeatherActionViewModel() : base("asv:shell.page.map.action.weather")
    {
        if (Design.IsDesignMode)
        {
            Visibility = true;
            
            CurrentWeatherData = new WeatherData
            {
                WindSpeed = 5,
                WindDirection = 123,
                Temperature = 295.25
            };

            WindDirection = 20;
            WindSpeedString = $"{CurrentWeatherData.WindSpeed} m/s";
            Temperature = $"{CurrentWeatherData.Temperature} K";
        }
    }
    
    [ImportingConstructor]
    public WeatherActionViewModel(IWeatherService weatherService, ILocalizationService localizationService) : this()
    {
        _weatherService = weatherService;
        _localizationService = localizationService;

        _weatherService.LastWeatherData
            .Subscribe(_ => CurrentWeatherData = _)
            .DisposeItWith(Disposable);
        
        _weatherService.Visibility
            .Subscribe(_ => Visibility = _)
            .DisposeItWith(Disposable);
    }
    
    public ICommand UpdateWeather { get; set; }
    
    [Reactive]
    public WeatherData CurrentWeatherData { get; set; }
    
    [Reactive]
    public string WindSpeedString { get; set; }
    
    [Reactive]
    public double WindSpeed { get; set; }

    [Reactive]
    public double WindDirection { get; set; }
    
    [Reactive]
    public string Temperature { get; set; }

    [Reactive]
    public bool Visibility { get; set; }
    
    public async Task UpdateWeatherImpl(GeoPoint location)
    {
        CurrentWeatherData = await _weatherService.GetWeatherData(location);
    }

    
    public override IMapAction Init(IMap context)
    {
        UpdateWeather = ReactiveCommand.CreateFromTask(
            () => UpdateWeatherImpl(context.Center)).DisposeItWith(Disposable);
        
        this.WhenPropertyChanged(_ => _.CurrentWeatherData)
            .Subscribe(_ =>
            {
                if (CurrentWeatherData != null)
                {
                    WindDirection = CurrentWeatherData.WindDirection;
                    WindSpeed = CurrentWeatherData.WindSpeed;
                    WindSpeedString = _localizationService.Velocity.FromSiToStringWithUnits(CurrentWeatherData.WindSpeed);
                    Temperature = _localizationService.Temperature.FromSiToStringWithUnits(CurrentWeatherData.Temperature);

                    _weatherService.LastWeatherData.OnNext(CurrentWeatherData);
                }
            })
            .DisposeItWith(Disposable);
        
        return this;
    }
}