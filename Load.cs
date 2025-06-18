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

		double SwayAcc = 0, SwayRate = 0, SpeedKmh = 0, Gtot = 0;
		bool Paused = false;
		uint Gct = 0;

		// `OverSteer()` ignores steering; just sorts vehicle orientation vs trajectory
		// Since radians per second can be well outside meaningful static radian ranges,
		// Atan() calculations would be bizarre.

		// Practically, if YawRate is sampled often enough
		// that angular changes between samples are < 12 degrees or 0.2 radians,
		// and since tan(0.2) is nearly 0.2, then dividing radians per second by tangential speed
		// is practically equivalent to lateral displacement increments divided by tangential distance increments.

		// Also practically, SwayRate (`1000 * AccelerationSway / SpeedKmh`) by observation
		// correlates directly to `OrientationYawVelocity` *without dividing* `OrientationYawVelocity` by `SpeedKmh`.
		double OldOS = 0;
		internal double OverSteer()
		{
			if (Paused)
                return OldOS;

            // division blows up near 0 YawRate;  avoid Yaw and Sway of different signs
            if (5000 > Gct && 9 > YawRate * YawRate && 25 > SwayRate * SwayRate && 1 > SwayAcc * SwayAcc
				&& 1 > KSwayAcc * KSwayAcc && 0 != YawRate && 0 < SwayRate * YawRate)
			{	// relatively small YawRate / SwayRate are nearly linear
            	LPfilter(ref LPyaw, 10, YawRate);
				LPfilter(ref LPsway, 10, SwayRate);
				double scale = 10 * LPsway / LPyaw;		// sometimes still negative!!
				if (1 < scale && 90 > scale)
				{
					Gct++;								// average estimated scale factor
					Gtot += scale;
//					int iscale = (int)(0.5 + Gtot / Gct);
//					if (iscale != View.Model.OverScale)
//					{
//						oops = $"OverSteer() Gct {Gct}";
						View.Model.OverScale = (int)(0.5 + Gtot / Gct);
//					}
				}
//				else oops = $"OverSteer() scale {scale}";
			}
			return OldOS = Math.Abs(0.1 * View.Model.OverScale * YawRate) - Math.Abs(SwayRate);
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
