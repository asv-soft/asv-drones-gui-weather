using System.ComponentModel.Composition;
using Asv.Drones.Gui.Core;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace Asv.Drones.Gui.Weather;

[PluginEntryPoint("Weather", CorePlugin.Name)]
[PartCreationPolicy(CreationPolicy.Shared)]
public class WeatherPlugin : IPluginEntryPoint
{
    [ImportingConstructor]
    public WeatherPlugin()
    {
        
    }
    public void Initialize()
    {
        // Application.Current
        //     .Styles.Add(new StyleInclude(new Uri("avares://Asv.Drones.Gui.Weather/Controls/WindIndicator.axaml")));
    }

    public void OnFrameworkInitializationCompleted()
    {
         
    }

    public void OnShutdownRequested()
    {
        
    }
}