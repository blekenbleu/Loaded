# Loaded
*SimHub plugin for loaded slip/grip experiments*  

- *ported from [SimHubPluginSdk](https://github.com/blekenbleu/SimHubPluginSdk)*  
Determine lateral and longitudinal accelerations (grip) and velocities.  

Estimate vertical wheel loads comparable to Assetto Corsa properties  
Assumption:  AccelerationHeave is strongly related to total wheel load  
- with car at rest `AccelerationHeave == 0`  
- peak negative AccelerationHeave is around 25, presumably force of gravity when cars fall...  

Among games, only Assetto Corsa seems to directly report per-wheel loads  
- SimHub also shows those properties for ACC, but always zero...  
by observation in AC, wheel load correlates well with suspension travel  
- wheel loads have higher peaks than travel by impact of dampers on spring linearity...

Proposed new wheel load estimation:  
- Distribute `AccelerationHeave` over wheels, based on ratio of    
	`(per-wheel suspension travel) / (sum of suspension travels)`

Example:  
Front Right wheel load:   `loadFR = 25 + Loaded.Heave*Loaded.FRdefl/(Loaded.FRdefl + Loaded.FLdefl + Loaded.RRdefl + Loaded.RLdefl)`
- different games have different property names for suspension travel;  
	unclear whether any iRacing properties correspond to suspension travel...

#### Ratio for Load vs suspension deflection is quite nonlinear between compression and unloading
- heave plot looks similar to sum of wheel load deltas
	- also similar to sum of wheel deflection deltas...
- use +/- heave to separately calibrate +/- suspension deflection deltas to estimated load scale factors
	- heave minimum should be -25;&nbsp; maximum may be > 50
	- load minimum should be 0...

### [Oversteer](https://github.com/blekenbleu/Loaded/blob/main/Oversteer.md#oversteer)
- one version based on [RangeyRover Automobilista 2 ShakeIt profile JavaScript](Properties/RearLeftFormula.md)
- another considering only vehicle attitude vs trajectory

### [Slip](https://blekenbleu.github.io/SimHub/slip.htm)
- Oversteer is based on slip, specifically [center-of-gravity SideSlip AKA lateral velocity](https://blekenbleu.github.io/SimHub/sideslip.htm)
- start from `SideSlip rate =  AccelerationSway - OrientationYawVelocity * SpeedKmh`,  
 &emsp; then integrate over time

### relevant SimHub properties
- AccelerationHeave, AccelerationSurge, AccelerationSway,
 GlobalAccelerationG, OrientationPitchAcceleration,
 OrientationRollAcceleration, OrientationYawAcceleration,
 OrientationYawVelocity, SpeedKmh
- GlobalAccelerationG just is the negative of AccelerationSurge

### [motion formulae, approximations](https://blekenbleu.github.io/SimHub/slip.htm#formulae)

## Observations
- Acceleration Sway and OrientationYawVelocity are comparable
	- if scaled approx 1:2, [**differences estimate understeer and oversteer**](https://blekenbleu.github.io/SimHub/Oversteer)
	- added Gain adjustment to adaptively match small Sway and Yaw amplitudes
	- try to ignore spikes observed in SimHub `OrientationYawVelocity` property plots
	- save and restore all slider values
- at least in AC and ACC, *positive* AccelerationSurge is *deceleration*...?

### New to me: *TwoWay Binding*
- XAML:&nbsp; `Value="{Binding foo, Mode=TwoWay}"` 
- `DataContext` can be per-XAML element...
- Setting `DataContext` inside .xaml did not work for `TwoWay`:
```
    <UserControl.DataContext>
        <local:Model/>
    </UserControl.DataContext>
```

-  `TwoWay` *works* for code-behind:  
```
        public Control(Loaded plugin) : this()
        {
            Plugin = plugin;
            DataContext = Model = new Model();
        }
```
- *reminder*:&nbsp; [**WPF Data Binding: C# INotifyPropertyChanged**](https://wellsb.com/csharp/learn/wpf-data-binding-csharp-inotifypropertychanged/)

### New to me:
- *range slider* - lower and upper thumbs, logarithmic range,
[modified from WpfRangeSlider](https://github.com/blekenbleu/WpfRangeSlider)  
![](https://github.com/blekenbleu/WpfRangeSlider/raw/master/Blue.jpg)  
- [xaml Binding Converter](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-convert-bound-data)  
```
    // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-convert-bound-data
    [ValueConversion(typeof(double), typeof(double))]
    public class PerCentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 100D * (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0.01 * (double)value;
        }
    }
```
 &emsp; For SimHub plugins with xaml `ResourceDictionary` (e.g. for RangeSlider),  
 &emsp; [Binding Converter instance declaration](https://riptutorial.com/xaml/example/29208/creating-and-using-a-converter--booleantovisibilityconverter-and-invertiblebooleantovisibilityconverter) (`x:Key`) goes *in it*:
```
        <UserControl.Resources>
            <ResourceDictionary>
                <loaded:PerCentConverter x:Key="percentConverter"/>
                <ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary Source="Themes/RangeSlider.xaml"/>
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </UserControl.Resources>
...
	<TextBox Name="loPC" Text="{Binding Path=Stlo, Converter={StaticResource percentConverter}}"/>
```
