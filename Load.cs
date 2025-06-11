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
				Defl = DeflFL + DeflFR + DeflRL + DeflRR;
				if (null != LoadStr)
					Loads = LoadFL + LoadFR + LoadRL + LoadRR;
			}
		}

		double oldyaw = 0;
		internal double DiffYawSway(double yaw, double sway)
		{
			double ayaw = Math.Abs(yaw);
			double asway = Math.Abs(sway);
			if (ayaw > 2 * asway)
				return Math.Abs(oldyaw) - asway;
			oldyaw = yaw;
			if (4 > asway)
			{
				LPfilter(ref LPyaw, 10, yaw);
				LPfilter(ref LPsway, 10, sway);
				LPdiff = LPyaw - LPsway;
				if (1 < LPdiff || -1 > LPdiff)
				{
					double gain = View.Model.YawVelGain * LPsway / LPyaw;
					if (1 < gain && 90 > gain)
						View.Model.YawVelGain = (int)(0.5 * gain);
				}
			}
			return ayaw - asway;
		}

		internal double LPfilter(ref double dold, double factor, double dnew)
		{
			dold += (dnew - dold) / factor;
			return dold;
		}

		internal double HPfilter(ref double dold, double factor, double dnew)
		{
			dold += (dnew - dold) / factor;
			return dnew - dold;
		}
	}
}
