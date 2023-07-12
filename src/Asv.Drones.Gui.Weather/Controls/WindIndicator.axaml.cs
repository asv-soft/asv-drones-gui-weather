using Asv.Drones.Gui.Core;
using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using Color = Avalonia.Media.Color;

namespace Asv.Drones.Gui.Weather;

public class WindIndicator : IndicatorBase
{
    #region Styled Props
    public static readonly StyledProperty<double> MaxValueProperty = AvaloniaProperty.Register<WindIndicator, double>(
        nameof(MaxValue), 10);

    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }
    
    public static readonly StyledProperty<SolidColorBrush> CurrentColorProperty = AvaloniaProperty.Register<WindIndicator, SolidColorBrush>(
        nameof(CurrentColor), new SolidColorBrush(new Color(255, 255, 255, 255)));

    public SolidColorBrush CurrentColor
    {
        get => GetValue(CurrentColorProperty);
        set => SetValue(CurrentColorProperty, value);
    }
    #endregion

    #region Value
    private double _value;

    public static readonly DirectProperty<WindIndicator, double> ValueProperty = AvaloniaProperty.RegisterDirect<WindIndicator, double>(
        nameof(Value), o => o.Value, (o, v) => o.Value = v);

    /// <summary>
    /// Wind speed value 
    /// </summary>
    public double Value
    {
        get => _value;
        set => SetAndRaise(ValueProperty, ref _value, value);
    }
    #endregion
    
    #region Angle
    private  double _angle;

    public static readonly DirectProperty<WindIndicator, double> AngleProperty = AvaloniaProperty.RegisterDirect<WindIndicator, double>(
        nameof(Angle), o => o.Angle, (o, v) => o.Angle = v);

    /// <summary>
    /// Wind direction angle 
    /// </summary>
    public double Angle
    {
        get => _angle;
        set => SetAndRaise(AngleProperty, ref _angle, value);
    }
    #endregion
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty || change.Property == MaxValueProperty)
        {
            var value = Value;

            if (value >= MaxValue)
            {
                CurrentColor = new SolidColorBrush(new Color(255, 255, 0, 0));
            }
            else
            {
                var nearestValue = (byte)(byte.MaxValue -(255 * value / MaxValue));
            
                CurrentColor = new SolidColorBrush(new Color(255, 255, nearestValue, nearestValue));    
            }
            
        }
    }
    
    public WindIndicator()
    {
        
    }
}