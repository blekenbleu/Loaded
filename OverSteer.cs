using System;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		uint Gct = 0, Sct = 0;
		double YawSway = 0;

		// `OverSteer()` ignores steering; just sorts vehicle orientation vs trajectory
		// Since radians per second can be well outside meaningful static radian ranges,
		// Atan() calculations would be bizarre.

		// Practically, if YawRate is sampled often enough
		// that angular changes between samples are < 12 degrees or 0.2 radians,
		// and since tan(0.2) is nearly 0.2, then dividing radians per second by tangential speed
		// is practically equivalent to lateral displacement increments divided by tangential distance increments.

		// Also practically, SwayRate (`1000 * AccelerationSway / SpeedKmh`) by observation
		// correlates directly to `OrientationYawVelocity` *without dividing* `OrientationYawVelocity` by `SpeedKmh`.
		internal double OverSteer()
		{
			double scale100;			// sliders are approx 100x unity

			if (5 > SpeedKmh)
				return YawSway = 0;

			// division blows up near 0 YawRate;  avoid Yaw and Sway of different signs
			if (5000 > Gct && 9 > YawRate * YawRate && 25 > SwayRate * SwayRate && 1 > SwayAcc * SwayAcc
				&& 1 > KSwayAcc * KSwayAcc && 0 != YawRate && 0 <= SwayRate * YawRate && 0 < SwayRate * Steering)
			{   // relatively small YawRate / SwayRate are nearly linear
				// rescale yaw and sway to approximate Steering range
				LPfilter(ref LPyaw, 10, 0.25 * YawRate);  // opposite sign from Steering
				LPfilter(ref LPsway, 10, 0.1 * SwayRate);// opposite sign from Steering;

				// rescale for sliders
				scale100 = 100 * Steering / LPyaw;      // sometimes still negative!!
				if (1 < scale100 && 190 > scale100)
				{
					Gct++;                              // average estimated scale factor
					Gtot += scale100;
					//					int iscale = (int)(0.5 + Gtot / Gct);
					//					if (iscale != View.Model.YawScale)
					//					{
					//						oops = $"OverSteer() Gct {Gct}";
					View.Model.YawScale = (int)(0.5 + Gtot / Gct);
					//					}
				}
				else oops = $"OverSteer() LPyaw scale {scale100}";

				scale100 = 100 * Steering / LPsway;
				if (1 < scale100 && 190 > scale100)
				{
					Sct++;                            // average estimated scale factor
					Stot += scale100;
					View.Model.SwayScale = (int)(0.5 + Stot / Sct);
				}
				else oops = $"OverSteer() LPsway scale {scale100}";
			}
			// remove slider 100x but apply YawRate and SwayRate scale factors to match Steering degrees
			LPfilter(ref YawSway, 10, 0.0025 * View.Model.YawScale * YawRate - 0.001 * View.Model.SwayScale * SwayRate);
			return Math.Abs(YawSway) - Math.Abs(Steering + YawSway);
		}
	}
}
