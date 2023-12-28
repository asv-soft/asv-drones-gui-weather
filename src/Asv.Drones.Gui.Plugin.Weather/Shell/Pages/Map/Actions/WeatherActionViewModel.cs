using System.ComponentModel.Composition;
using System.Windows.Input;
using Asv.Common;
using Asv.Drones.Gui.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Plugin.Weather;

/// <summary>
/// Represents a view model for the weather action on a map.
/// </summary>
[Export(FlightPageViewModel.UriString,typeof(IMapAction))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WeatherActionViewModel : MapActionBase
{
    /// <summary>
    /// Represents a weather service used to retrieve weather information.
    /// </summary>
    private readonly IWeatherService _weatherService;

    /// <summary>
    /// Represents a service for handling localization.
    /// </summary>
    private readonly ILocalizationService _localizationService;

    /// This class represents a view model for the weather action in the application.
    /// /
    public WeatherActionViewModel() : base("asv:shell.page.map.action.weather")
    {
        if (Design.IsDesignMode)
        {
            Application.Current.Styles.Add(new StyleInclude(new Uri("resm:Styles?assembly=Asv.Drones.Gui.Plugin.Weather"))
            {
                Source = new Uri("avares://Asv.Drones.Gui.Plugin.Weather/App.axaml")
            });
            
            Visibility = true;
            
            CurrentWeatherData = new WeatherData
            {
                WindSpeed = 8,
                WindDirection = 123,
                Temperature = 22.25
            };

            WindDirection = CurrentWeatherData.WindDirection;
            WindSpeedString = $"{CurrentWeatherData.WindSpeed} m/s";
            Temperature = $"{CurrentWeatherData.Temperature} K";
        }
    }

    /// <summary>
    /// Initializes a new instance of the WeatherActionViewModel class with the specified weather service and localization service.
    /// </summary>
    /// <param name="weatherService">The weather service to be used for retrieving weather data.</param>
    /// <param name="localizationService">The localization service to be used for retrieving localization data.</param>
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

    /// <summary>
    /// Gets or sets the command to update the weather.
    /// </summary>
    public ICommand UpdateWeather { get; set; }

    /// <summary>
    /// Gets or sets the current weather data.
    /// </summary>
    /// <value>
    /// The current weather data.
    /// </value>
    /// <remarks>
    /// This property will be updated with the latest weather data in a reactive manner.
    /// </remarks>
    [Reactive]
    public WeatherData CurrentWeatherData { get; set; }

    /// <summary>
    /// Gets or sets the string representation of the wind speed.
    /// </summary>
    /// <remarks>
    /// This property is decorated with the <see cref="Reactive"/> attribute.
    /// The wind speed string provides information about the wind speed in a textual format.
    /// It can be used to display the wind speed to the user, without the need for further conversion or formatting.
    /// The string may represent the wind speed in different units, such as kilometers per hour (km/h) or miles per hour (mph).
    /// </remarks>
    [Reactive]
    public string WindSpeedString { get; set; }

    /// <summary>
    /// Gets or sets the wind speed.
    /// </summary>
    [Reactive]
    public double WindSpeed { get; set; }

    /// <summary>
    /// The current wind direction in degrees.
    /// </summary>
    [Reactive]
    public double WindDirection { get; set; }

    /// <summary>
    /// Gets or sets the temperature.
    /// </summary>
    /// <remarks>
    /// The temperature property represents the current temperature as a string.
    /// </remarks>
    /// <value>
    /// A string that represents the current temperature.
    /// </value>
    [Reactive]
    public string Temperature { get; set; }

    /// <summary>
    /// Gets or sets the Visibility of the property
    /// </summary>
    /// <remarks>
    /// The Visibility property indicates whether the object is visible or not.
    /// </remarks>
    /// <value>
    /// True if the object is visible; otherwise, false.
    /// </value>
    [Reactive]
    public bool Visibility { get; set; }

    /// <summary>
    /// Updates the current weather data for a given location.
    /// </summary>
    /// <param name="location">The location for which to update the weather data.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateWeatherImpl(GeoPoint location)
    {
        CurrentWeatherData = await _weatherService.GetWeatherData(location);
    }


    /// Initializes the map action with the given map context.
    /// @param context The map context for the action.
    /// @returns Returns this initialized map action.
    /// /
    public override IMapAction Init(IMap context)
    {
        base.Init(context);
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