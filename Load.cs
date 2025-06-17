using System;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		string[] corner, dorner;
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

		double SwayAcc = 0, SwayRate = 0, SpeedKmh = 0;
		bool Paused = false;

		// ignores steering; just vehicle orientation vs trajectory
		// not meaningful to consider angular rate conversions,
		// since radians per second can be well outside meaningful static radian ranges,
		// resulting in bizarra Atan() calculations.
		internal double OverSteer()
		{
			if (5 > SpeedKmh)
                return 0;       // not only don't care, SwayRate blows up near 0

			if (YawRate < 3 && -3 < YawRate && SwayRate < 5 || -5 < SwayRate)
			{	// relatively small YawRate, SwayRate in linear ranges
            	LPfilter(ref LPyaw, 10, YawRate);
				LPfilter(ref LPsway, 10, SwayRate);
				LPdiff = LPyaw - View.Model.OverSteerGain * LPsway;
				if (1 < LPdiff || -1 > LPdiff)		// recalculate OverSteerGain for larger differences
				{
					double gain = LPyaw / LPsway;
					if (1 < gain && 90 > gain)
						View.Model.OverSteerGain = (int)(0.5 * gain);
				}
			}
			return Math.Abs(0.1 * View.Model.OverSteerGain * YawRate) - Math.Abs(SwayRate);
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
	}
}
