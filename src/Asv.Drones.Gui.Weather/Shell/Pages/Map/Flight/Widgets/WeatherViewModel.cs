using System.ComponentModel.Composition;
using Asv.Drones.Gui.Core;

namespace Asv.Drones.Gui.Weather;

[Export()]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WeatherViewModel : FlightWidgetBase
{
    public WeatherViewModel() : base(new(UriString + ".weather"))
    {
        Location = WidgetLocation.Bottom;
    }
    
    [ImportingConstructor]
    public WeatherViewModel(IWeatherService svc) : this()
    {
        
    }
    
    protected override void InternalAfterMapInit(IMap context)
    {
        
    }
}