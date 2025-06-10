# Loaded
- *ported from [SimHubPluginSdk](https://github.com/blekenbleu/SimHubPluginSdk)*  

Estimate SimHub wheel loads comparable to Assetto Corsa properties  
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

## Observations
- Acceleratioin Sway and OrientationYawVelocity are comparable
	- if scaled approx 1:2, differences estimate understeer and oversteer
	- added YawVelocity Gain to adaptively match small Sway and Yaw amplitudes
	- rescaled YawVelocity tries to ignore spikes in SimHub OrientationYawVelocity values
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
