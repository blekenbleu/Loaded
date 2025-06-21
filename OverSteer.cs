using System;
using System.Windows;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		ushort Gct = 0, Sct = 0;
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
		double OS ()
		{
			// remove slider 100x but apply YawRate and SwayRate scale factors to match Steering degrees
			var yaw = 0.0025 * View.Model.YawScale * YawRate;
			var sway = 0.001 * View.Model.SwayScale * SwayRate;
			LPfilter(ref YawSway, 10, yaw - sway);
			var os = Math.Abs(YawSway) - Math.Abs(Steering + YawSway);
			return os;
		}

		internal double OverSteer()
		{
			double scale100;			// sliders are approx 100x unity
			Srun = 0;
			if (Gct < 4999 && Visibility.Visible == View.Model.ButtonVisibility && "Green" == View.Model.ModeColor)
				View.Model.ModeColor = "Red";

			if (5 > SpeedKmh)
				return YawSway = 0D;

			// division blows up near 0 YawRate
			if (5000 <= Gct || 0D == YawRate)
				return OS();
 			if ((9F < YawRate * YawRate) || (25F < SwayRate * SwayRate))
				return OS();
 			if (1F < SwayAcc * SwayAcc)
				return OS();
			// avoid Steering, Yaw and Sway of different signs
			if (Math.Sign(SwayRate) != Math.Sign(YawRate))
				return OS();
			if (Math.Sign(SwayRate) != Math.Sign(Steering))
				return OS();

			// relatively small YawRate / SwayRate are nearly linear
			// rescale yaw and sway to approximate Steering range
			var Absteer = Math.Abs(100 * Steering);
			LPfilter(ref LPyaw, 10, Math.Abs(0.25D * YawRate));	// opposite sign from Steering
			LPfilter(ref LPsway, 10, Math.Abs(0.1D * SwayRate));// opposite sign from Steering;

			// rescale for sliders
			scale100 = Absteer / LPyaw;		// sometimes still negative!!
			if (1 < scale100 && 190 > scale100)
			{
				Gct++;								// average estimated scale factor
				Gtot += scale100;
				//					int iscale = (int)(0.5 + Gtot / Gct);
				//					if (iscale != View.Model.YawScale)
				//					{
				//						oops = $"OverSteer() Gct {Gct}";
				View.Model.YawScale = (int)(0.5 + Gtot / Gct);
				//					}
			}
			else oops = $"OverSteer() LPyaw scale {scale100}";

			scale100 = Absteer / LPsway;
			if (1 < scale100 && 190 > scale100)
			{
				Sct++;							// average estimated scale factor
				Stot += scale100;
				View.Model.SwayScale = (int)(0.5 + Stot / Sct);
			}
			else oops = $"OverSteer() LPsway scale {scale100}";
			return OS();
		}
	}
}
