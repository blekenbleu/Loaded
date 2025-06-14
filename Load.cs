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

		double SwayAcc = 0, SpeedKmh = 0, slipAngle = 0;
		bool Paused = false;

		// ignores steering; just vehicle orientation vs trajectory
		internal double SlipAngle()
		{
			if (5 > SpeedKmh)
                return 0;       // not only don't care, also blows up near 0

			double swayrate, ayaw = Math.Abs(YawRate);
			// sway rate converted to radians
			double asway = Math.Abs(swayrate = Math.Atan(1000 * SwayAcc / SpeedKmh));

			slipAngle = ayaw - View.Model.SlipGain * asway;
			if (ayaw > 20 || asway > 1)
				return slipAngle;

			LPfilter(ref LPyaw, 10, YawRate);
			LPfilter(ref LPsway, 10, swayrate);
			LPdiff = LPyaw - View.Model.SlipGain * LPsway;
			if (1 < LPdiff || -1 > LPdiff)
			{
				double gain = LPyaw / LPsway;
				if (1 < gain && 90 > gain)
				{
					View.Model.SlipGain = (int)(0.5 * gain);
					slipAngle = ayaw - View.Model.SlipGain * asway;
				}
			}
			return slipAngle;
		}

		internal void LPfilter(ref double dold, double factor, double dnew)
		{
			if (!Paused)	// avoid contaminating filtered data during pauses
				dold += (dnew - dold) / factor;
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
