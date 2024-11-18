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
				FiltFL = Filter(FiltFL, View.Model.Filter_L, LoadFL);
				FiltFR = Filter(FiltFR, View.Model.Filter_L, LoadFR);
				FiltRL = Filter(FiltRL, View.Model.Filter_L, LoadRL);
				FiltRR = Filter(FiltRR, View.Model.Filter_L, LoadRR);
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
				if (100 > Zero && View.Model.Thresh_sv > LSpeed
				 && View.Model.Thresh_sh * 0.01 > Math.Abs(Heave)
				 && View.Model.Thresh_ss * 0.01 > Math.Abs(LSurge))
				{
					Zero++;
					Defl0[0] += DeflFL;
					Defl0[1] += DeflFR;
					Defl0[2] += DeflRL;
					Defl0[3] += DeflRR;
					Defl0Avg[0] = Defl0[0] / Zero;
					Defl0Avg[1] = Defl0[1] / Zero;
					Defl0Avg[2] = Defl0[2] / Zero;
					Defl0Avg[3] = Defl0[3] / Zero;
					if (null != LoadStr)
					{
						Load0[0] += LoadFL;
						Load0[1] += LoadFR;
						Load0[2] += LoadRL;
						Load0[3] += LoadRR;
						Load0Avg[0] = Load0[0] / Zero;
						Load0Avg[1] = Load0[1] / Zero;
						Load0Avg[2] = Load0[2] / Zero;
						Load0Avg[3] = Load0[3] / Zero;
					}
				}
				if (0 < Zero)
				{
					EstFL = Load0Avg[0] * (1 - 0.1 * Settings.Gain * (1 - DeflFL / Defl0Avg[0]));
					EstFR = Load0Avg[1] * (1 - (1 - 0.1 * Settings.Gain * DeflFR / Defl0Avg[1]));
					EstRL = Load0Avg[2] * (1 - (1 - 0.1 * Settings.Gain * DeflRL / Defl0Avg[2]));
					EstRR = Load0Avg[3] * (1 - (1 - 0.1 * Settings.Gain * DeflRR / Defl0Avg[3]));
				}
			}
		}

		double Filter(double dold, double factor, double dnew)
		{
			return dold + (dnew - dold) / factor;
		}
	}
}
