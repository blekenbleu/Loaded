using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace blekenbleu.loaded
{
	public partial class Loaded : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		public void Load()
		{
			if (null != LoadStr)	// few games have load properties
			{
				if (null != corner)
				{
					LoadFR = Convert.ToDouble(pm.GetPropertyValue(LoadStr+corner[0]));
					LoadFL = Convert.ToDouble(pm.GetPropertyValue(LoadStr+corner[1]));
					LoadRR = Convert.ToDouble(pm.GetPropertyValue(LoadStr+corner[2]));
					LoadRL = Convert.ToDouble(pm.GetPropertyValue(LoadStr+corner[3]));
				}
				else {
					LoadFR = Convert.ToDouble(pm.GetPropertyValue(LoadStr+dorner[0]));
					LoadFL = Convert.ToDouble(pm.GetPropertyValue(LoadStr+dorner[1]));
					LoadRR = Convert.ToDouble(pm.GetPropertyValue(LoadStr+dorner[2]));
					LoadRL = Convert.ToDouble(pm.GetPropertyValue(LoadStr+dorner[3]));
				}
                Loads = LoadFL + LoadFR + LoadRL + LoadRR;
            }
            if (null != DeflStr)	// not all games handled in Init()
			{
				if (null != dorner)
				{
					DeflFR = Convert.ToDouble(pm.GetPropertyValue(DeflStr+dorner[0]));
					DeflFL = Convert.ToDouble(pm.GetPropertyValue(DeflStr+dorner[1]));
					DeflRR = Convert.ToDouble(pm.GetPropertyValue(DeflStr+dorner[2]));
					DeflRL = Convert.ToDouble(pm.GetPropertyValue(DeflStr+dorner[3]));
				}
				else {
					DeflFR = Convert.ToDouble(pm.GetPropertyValue(DeflStr+corner[0]));
					DeflFL = Convert.ToDouble(pm.GetPropertyValue(DeflStr+corner[1]));
					DeflRR = Convert.ToDouble(pm.GetPropertyValue(DeflStr+corner[2]));
					DeflRL = Convert.ToDouble(pm.GetPropertyValue(DeflStr+corner[3]));
				}
			}
		}

		double oldyaw = 0, SwayAcc = 0, SpeedKmh = 0;
		internal double DiffYawSway()
		{
			double ayaw = Math.Abs(YawVel);
			double asway = Math.Abs(SwayAcc);
			if (ayaw > 20 || asway > 1)
				return Math.Abs(oldyaw) - asway;
			oldyaw = YawVel;
			if (4 > asway)
			{
				LPfilter(ref LPyaw, 10, YawVel);
				LPfilter(ref LPsway, 10, SwayAcc);
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

		double Slip(char corner)
		{
			string raw = "DataCorePlugin.GameRawData.Physics.WheelSlip0" + corner;
			var slip = pm.GetPropertyValue(raw);
			return (null == slip) ? 0 : Convert.ToDouble(slip);
		}
	}
}
