using System.ComponentModel.Composition;
using Asv.Drones.Gui.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Plugin.Weather;

[ExportView(typeof(WeatherSettingsViewModel))]
[PartCreationPolicy(CreationPolicy.NonShared)]
public partial class WeatherSettingsView : ReactiveUserControl<WeatherSettingsViewModel>
{
    public WeatherSettingsView()
    {
        InitializeComponent();
    }
}