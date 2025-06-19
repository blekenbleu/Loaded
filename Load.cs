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

		bool Paused = false;
		double SwayAcc = 0, SwayRate = 0, SpeedKmh = 0, Gtot = 0, Stot = 0;

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
