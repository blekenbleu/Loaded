using GameReaderCommon;
using SimHub.Plugins;
using System;

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
					CarId = data.NewData.CarId;
				}
				Defl = DeflFL + DeflFR + DeflRL + DeflRR;
				if (null != LoadStr)
					Loads = LoadFL + LoadFR + LoadRL + LoadRR;
			}
		}

		internal double Filter(double dold, double factor, double dnew)
		{
			return dold + (dnew - dold) / factor;
		}
	}
}
