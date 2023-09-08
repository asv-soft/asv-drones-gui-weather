using System.ComponentModel.Composition;
using Asv.Drones.Gui.Core;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Weather;

[ExportView(typeof(WeatherActionViewModel))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public partial class WeatherActionView : ReactiveUserControl<WeatherActionViewModel>
{
    public WeatherActionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}