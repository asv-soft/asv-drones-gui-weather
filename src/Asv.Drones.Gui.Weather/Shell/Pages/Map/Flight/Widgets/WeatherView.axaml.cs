using System.ComponentModel.Composition;
using Asv.Drones.Gui.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Asv.Drones.Gui.Weather;

[ExportView(typeof(WeatherViewModel))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public partial class WeatherView : UserControl
{
    public WeatherView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}