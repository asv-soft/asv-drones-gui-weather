using System.ComponentModel.Composition;
using Asv.Drones.Gui.Core;
using DynamicData;

namespace Asv.Drones.Gui.Weather;

[Export(FlightPageViewModel.UriString, typeof(IViewModelProvider<IMapWidget>))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public class WeatherWidgetProvider : ViewModelProviderBase<IMapWidget>
{
    [ImportingConstructor]
    public WeatherWidgetProvider(WeatherViewModel weatherViewModel)
    {
        Source.AddOrUpdate(weatherViewModel);
    }
}