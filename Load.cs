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
					Defl0[0] = Defl0[1] = Zero = 0;	// car suspension deflections differ
					CarId = data.NewData.CarId;
				}
				if (Thresh_sv > data.NewData.SpeedLocal && Thresh_sh > Math.Abs(Heave) && Thresh_ss * 0.01 > Math.Abs((double)data.NewData.AccelerationSurge))
				{
					Zero++;
					Defl0[0] += (DeflFR + DeflFL);
					Defl0[1] += (DeflRR + DeflRL);
					Defl0Avg[0] = Defl0[0] / Zero;
					Defl0Avg[1] = Defl0[1] / Zero;
				}
			}
		}
	}
}
