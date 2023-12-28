using Asv.Drones.Gui.Core;
using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using Color = Avalonia.Media.Color;

namespace Asv.Drones.Gui.Plugin.Weather;

/// <summary>
/// Represents a wind indicator control that displays wind speed and direction.
/// </summary>
public class WindIndicator : IndicatorBase
{
    #region Styled Props

    /// <summary>
    /// Gets or sets the maximum value for the WindIndicator.
    /// </summary>
    public static readonly StyledProperty<double> MaxValueProperty = AvaloniaProperty.Register<WindIndicator, double>(
        nameof(MaxValue), 10);

    /// <summary>
    /// Gets or sets the maximum value allowed for the property.
    /// </summary>
    /// <value>The maximum value.</value>
    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the current color of the wind indicator.
    /// </summary>
    /// <value>
    /// The current color of the wind indicator.
    /// </value>
    public static readonly StyledProperty<SolidColorBrush> CurrentColorProperty = AvaloniaProperty.Register<WindIndicator, SolidColorBrush>(
        nameof(CurrentColor), new SolidColorBrush(new Color(255, 255, 255, 255)));

    /// <summary>
    /// Gets or sets the current color of the property.
    /// </summary>
    /// <value>
    /// The current color of the property.
    /// </value>
    /// <remarks>
    /// The <see cref="CurrentColor"/> property represents the current color of the property.
    /// This property uses the <see cref="GetValue(DependencyProperty)"/> and <see cref="SetValue(DependencyProperty, object)"/> methods
    /// to get and set the value of the <see cref="CurrentColorProperty"/>.
    /// </remarks>
    public SolidColorBrush CurrentColor
    {
        get => GetValue(CurrentColorProperty);
        set => SetValue(CurrentColorProperty, value);
    }
    #endregion

    #region Value

    /// <summary>
    /// Represents a private value of type double.
    /// </summary>
    private double _value;

    /// <summary>
    /// Gets or sets the value of the <see cref="WindIndicator"/>.
    /// </summary>
    public static readonly DirectProperty<WindIndicator, double> ValueProperty = AvaloniaProperty.RegisterDirect<WindIndicator, double>(
        nameof(Value), o => o.Value, (o, v) => o.Value = v);

    /// <summary>
    /// Gets or sets the wind speed value.
    /// </summary>
    /// <value>
    /// The wind speed value.
    /// </value>
    public double Value
    {
        get => _value;
        set => SetAndRaise(ValueProperty, ref _value, value);
    }
    #endregion
    
    #region Angle

    /// <summary>
    /// Represents the value of an angle.
    /// </summary>
    private  double _angle;

    /// <summary>
    /// Gets the dependency property for the angle of the wind indicator.
    /// </summary>
    public static readonly DirectProperty<WindIndicator, double> AngleProperty = AvaloniaProperty.RegisterDirect<WindIndicator, double>(
        nameof(Angle), o => o.Angle, (o, v) => o.Angle = v);

    /// <summary>
    /// Wind direction angle.
    /// </summary>
    public double Angle
    {
        get => _angle;
        set => SetAndRaise(AngleProperty, ref _angle, value);
    }
    #endregion

    /// <summary>
    /// Raises the PropertyChanged event and updates the CurrentColor property based on the changes.
    /// </summary>
    /// <param name="change">The AvaloniaPropertyChangedEventArgs object containing information about the property change.</param>
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

    /// <summary>
    /// Represents a WindIndicator object.
    /// </summary>
    public WindIndicator()
    {
        
    }
}