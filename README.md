# Loaded
- *ported from [SimHubPluginSdk](https://github.com/blekenbleu/SimHubPluginSdk)*  

Estimate SimHub wheel loads comparable to Assetto Corsa properties  
Assumption:  AccelerationHeave is strongly related to total wheel load  
- with car at rest `AccelerationHeave == 0`  
- peak negative AccelerationHeave is around 25, presumably force of gravity when cars fall...  

Among games, only Assetto Corsa seems to directly report per-wheel loads  
- ACC also has those properties, but always zero...  
by observation in AC, wheel load correlates well with suspension travel  
- wheel loads have higher peaks than travel by impact of dampers on spring linearity...

Proposed new wheel load estimation:  
- Distribute `AccelerationHeave` over wheels, based on ratio of    
	`(per-wheel suspension travel) / (sum of suspension travels)`

Example:  
Front Right wheel load:   `loadFR = 25 + AccelerationHeave*travelFR/(travelFR + travelFL + travelRR + travelRL)`  
- different games have different property names for suspension travel;  
	unclear whether any iRacing properties correspond to suspension travel...

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
