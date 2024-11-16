using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;

namespace blekenbleu.loaded
{
	public partial class Loaded : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		public void Load(PluginManager pluginManager, ref GameData data)
		{
			if (null != LoadStr)	// few games have load properties
			{
				if (null != corner)
				{
					LoadFR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[0]));
					LoadFL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[1]));
					LoadRR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[2]));
					LoadRL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[3]));
				}
				else {
					LoadFR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[0]));
					LoadFL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[1]));
					LoadRR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[2]));
					LoadRL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[3]));
				}
			}
			if (null != DeflStr)	// not all games handled in Init()
			{
				if (null != dorner)
				{
					DeflFR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[0]));
					DeflFL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[1]));
					DeflRR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[2]));
					DeflRL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[3]));
				}
				else {
					DeflFR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[0]));
					DeflFL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[1]));
					DeflRR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[2]));
					DeflRL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[3]));
				}
				if (CarId != data.NewData.CarId)
				{
					DeflF0 = DeflR0 = Zero = 0;	// car suspension deflections differ
					CarId = data.NewData.CarId;
				}
				if (3 > data.NewData.SpeedLocal && 0.05 > Math.Abs(Heave) && 0.15 > Math.Abs((double)data.NewData.AccelerationSurge))
				{
					Zero++;
					DeflF0 += (DeflFR + DeflFL);
					DeflR0 += (DeflRR + DeflRL);
					DeflF0Avg = DeflF0 / Zero;
					DeflR0Avg = DeflR0 / Zero;
				}
			}
		}
	}
}
