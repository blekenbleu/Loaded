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

### relevant SimHub properties
- AccelerationHeave, AccelerationSurge, AccelerationSway,
 GlobalAccelerationG, OrientationPitchAcceleration,
 OrientationRollAcceleration, OrientationYawAcceleration,
 OrientationYawVelocity, SpeedKmh
- GlobalAccelerationG just is the negative of AccelerationSurge

### motion formulae
- AccelerationSway = SpeedKmh * SpeedKmh / CurveRadius  
 &emsp; *Calculate CurveRadius from SimHub AccelerationSway and SpeedKmh*
- SideSlipAngle = SwayVelocity / SpeedKmh
 &emsp; (SwayVelocity aka lateral velocity)
- Circular velocity = OrientationYawVelocity * CurveRadius
- Since Circular velocity is basically SpeedKmh, then SpeedKmh = OrientationYawVelocity * CurveRadius  
or OrientationYawVelocity = SpeedKmh / CurveRadius;
- by substitution, AccelerationSway = SpeedKmh * OrientationYawVelocity  
- [side slip rate = AccelerationSway - SpeedKmh * OrientationYawVelocity](https://www.reddit.com/r/FSAE/comments/125moie/comment/je6it6q/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button);  
  &emsp; integrate side slip rate to obtain SideSlipAngle
- [AccelerationSway = SpeedKmh * (OrientationYawVelocity + side slip rate)](https://www.racetechlab.com/basics-of-cornering/#lateral-acceleration-the-key-metric)

### Approximations
- Tire slip angles less than 6 degrees are most useful.
- For angles less than 6 degrees, tan and arctan are nearly perfectly linear.
- Don't bother applying those functions.
- calculating slip angles for all 4 tires is wasteful;
 &emsp; slip angles mostly matter when large on heavily loaded side.

### Per-wheel Slip angle equations [*from OptimumG’s seminars*](https://optimumg.com/wp-content/uploads/2019/09/RCE3.pdf)
- SlipAngleLf = (LateralVelocity + YawVelocity * `Df`) / (SpeedKmh - YawVelocity * `Axf/2`) - SteeringAngleL  
- SlipAngleRf = (LateralVelocity + YawVelocity * `Df`) / (SpeedKmh + YawVelocity * `Axf/2`) - SteeringAngleR  
- SlipAngleLr = (LateralVelocity - YawVelocity * `Dr`) / (SpeedKmh - YawVelocity * `Axr/2`)  
- SlipAngleRr = (LateralVelocity - YawVelocity * `Dr`) / (SpeedKmh + YawVelocity * `Axr/2`)  
.. where:  
- `Df` and `Dr` are distances from center of gravity to front and rear axles, respectively  
- `Axf` and `Axr` are front and rear axle lengths, respectively  
- note sign changes front-to-rear (`Df` vs `Dr`) and left-to-right (`Axf` or `Axr`)

## Observations
- Acceleration Sway and OrientationYawVelocity are comparable
	- if scaled approx 1:2, [**differences estimate understeer and oversteer**](https://github.com/blekenbleu/Loaded/blob/main/Oversteer.md#oversteer)
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
