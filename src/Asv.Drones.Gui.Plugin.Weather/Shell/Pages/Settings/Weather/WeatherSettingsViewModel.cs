using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Asv.Common;
using Asv.Drones.Gui.Core;
using DynamicData.Binding;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Drones.Gui.Plugin.Weather;

/// <summary>
/// Represents the view model for the weather settings.
/// </summary>
[Export(typeof(ISettingsPart))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WeatherSettingsViewModel : SettingsPartBase
{
    private static readonly Uri Uri = new(SettingsPartBase.Uri, "weather");

    /// <summary>
    /// Represents a private readonly variable that stores an instance of the IWeatherService interface. </summary> <remarks>
    /// Use this variable to interact with the weather service and retrieve weather information. </remarks>
    /// /
    private readonly IWeatherService _weatherService;

    /// <summary>
    /// Represents a view model for weather settings.
    /// </summary>
    public WeatherSettingsViewModel() : base(Uri)
    {
        
    }

    /// A view model class for managing weather settings.
    /// /
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
        
        _weatherService.CurrentWeatherProviderApiKey.Subscribe(_ => CurrentWeatherProviderApiKey = _).DisposeItWith(Disposable);
        
        this.WhenAnyValue(_ => _.CurrentWeatherProviderApiKey)
            .Subscribe(_weatherService.CurrentWeatherProviderApiKey)
            .DisposeItWith(Disposable);
    }

    /// <summary>
    /// Gets the order of the property.
    /// </summary>
    /// <remarks>
    /// The order determines the position of the property relative to other properties.
    /// </remarks>
    /// <value>
    /// An <see cref="int"/> value representing the order of the property.
    /// </value>
    public override int Order => 4;

    /// <summary>
    /// Gets or sets the visibility of the property.
    /// </summary>
    /// <remarks>
    /// This property is reactive, meaning that changes to its value will trigger updates and notifications.
    /// </remarks>
    [Reactive]
    public bool Visibility { get; set; }

    /// <summary>
    /// Gets or sets the visibility button property.
    /// </summary>
    [Reactive]
    public string VisibilityButton { get; set; }

    /// <summary>
    /// Represents the current weather provider.
    /// </summary>
    /// <remarks>
    /// The CurrentWeatherProvider property is used to get or set the current weather provider for the application. The weather provider should implement the <see cref="IWeatherProvider
    /// Base"/> interface.
    /// </remarks>
    [Reactive]
    public IWeatherProviderBase CurrentWeatherProvider { get; set; }

    /// <summary>
    /// Gets or sets the API key used to authenticate the current weather provider.
    /// </summary>
    /// <remarks>
    /// This API key is required to establish a connection with the current weather provider and access their weather data.
    /// </remarks>
    /// <value>
    /// The current weather provider API key.
    /// </value>
    [Reactive]
    public string CurrentWeatherProviderApiKey { get; set; }

    /// <summary>
    /// Property representing the collection of weather providers. </summary> <remarks>
    /// This property provides access to the collection of weather providers used by the software. </remarks>
    /// <returns>An enumerable collection of objects implementing the IWeatherProviderBase interface.</returns>
    /// /
    public IEnumerable<IWeatherProviderBase> WeatherProviders => _weatherService.WeatherProviders;

    /// <summary>
    /// Gets the weather icon based on the current weather condition.
    /// </summary>
    /// <returns>
    /// A string representing the weather icon.
    /// </returns>
    public string WeatherIcon => MaterialIconDataProvider.GetData(MaterialIconKind.WeatherPartlyCloudy);

    /// <summary>
    /// The API key for retrieving weather icons.
    /// </summary>
    /// <value>
    /// A string representing the API key for retrieving weather icons.
    /// </value>
    public string WeatherApiKeyIcon => MaterialIconDataProvider.GetData(MaterialIconKind.Key);

    /// <summary>
    /// Gets the icon name for the weather switch.
    /// </summary>
    /// <remarks>
    /// The WeatherSwitchIcon property returns the icon name from the MaterialIconDataProvider.
    /// The icon represents the visibility status of the weather.
    /// Use this property to retrieve the icon name and display the appropriate icon.
    /// </remarks>
    /// <returns>The icon name for the weather switch.</returns>
    public string WeatherSwitchIcon => MaterialIconDataProvider.GetData(MaterialIconKind.Visibility);
}