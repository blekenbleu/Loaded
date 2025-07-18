using System;
using System.Windows;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		double YawSway = 0, YawVsteer = 0, SwayVsteer = 0, Absteer = 0;

		// `OverSteer()` sorts vehicle orientation vs trajectory
		// Since radians per second can be well outside meaningful static radian ranges,
		// Atan() calculations would be bizarre.

		// Practically, if YawVel is sampled often enough
		// that angular changes between samples are < 12 degrees or 0.2 radians,
		// and since tan(0.2) is nearly 0.2, then dividing radians per second by tangential speed
		// is practically equivalent to lateral displacement increments divided by tangential distance increments.

		double Limit(double limit, double rate)
		{
			if (rate > limit)
				return limit;
			if (rate < -limit)
				return -limit;
			return rate;
		}

		// Also practically, SwayRate (1000 * AccelerationSway / SpeedKmh) by observation
		// correlates directly to OrientationYawVelocity without dividing OrientationYawVelocity by SpeedKmh.
		double OS ()
		{
			// remove slider 100x but apply YawVel and SwayRate scale factors to match Steering degrees
			var yaw = 0.0025 * View.Model.YawScale * YawVel;
			var sway = 0.001 * View.Model.SwayScale * SwayRate;
			if (0.5 < Absteer)
			{
				YawVsteer  = Limit(2, yaw  / Steering);
				SwayVsteer = Limit(2, sway / Steering);
			}
			LPfilter(ref YawSway, 10, yaw - sway);
			var os = Math.Abs(YawSway) - Math.Abs(Steering + YawSway);
			return os;
		}

		internal double OverSteer()
		{
			double scale100;			// sliders are approx 100x unity
			Srun = 0;
			Absteer = Math.Abs(View.Model.SteerFact * Steering);

			if (Settings.Gct < 4999 && Visibility.Visible == View.Model.ButtonVisibility
				&& "Green" == View.Model.ModeColor)
				View.Model.ModeColor = "Red";

			if (5 > SpeedKmh)
				return YawSway = 0D;

			// division blows up near 0 YawVel
			if (5000 <= Settings.Gct || 0D == YawVel)
				return OS();
 			if ((9F < (YawVel * YawVel)) || (25F < (SwayRate * SwayRate)))
				return OS();
 			if (1F < (SwayAcc * SwayAcc))
				return OS();
			// avoid Steering, Yaw and Sway of different signs
			if (Math.Sign(SwayRate) != Math.Sign(YawVel))
				return OS();
			if (Math.Sign(SwayRate) != Math.Sign(Steering))
				return OS();

			// relatively small YawVel / SwayRate are nearly linear
			// rescale yaw and sway to approximate Steering range
			LPfilter(ref LPyaw, 10, Math.Abs(0.25D * YawVel));
			LPfilter(ref LPsway, 10, Math.Abs(0.1D * SwayRate));

			// rescale for sliders
			scale100 = Absteer / LPyaw;		// sometimes still negative!!
			if (1 < scale100 && 190 > scale100)
			{
				Settings.Gct++;								// average estimated scale factor
				Settings.Gtot += scale100;
				//					int iscale = (int)(0.5 + Settings.Gtot / Settings.Gct);
				//					if (iscale != View.Model.YawScale)
				//					{
				//						oops = $"OverSteer() Settings.Gct {Settings.Gct}";
				View.Model.YawScale = (int)(0.5 + Settings.Gtot / Settings.Gct);
				//					}
			}
			else oops = $"OverSteer() LPyaw scale {scale100}";

			scale100 = Absteer / LPsway;
			if (1 < scale100 && 190 > scale100)
			{
				Settings.Sct++;							// average estimated scale factor
				Settings.Stot += scale100;
				View.Model.SwayScale = (int)(0.5 + Settings.Stot / Settings.Sct);
			}
			else oops = $"OverSteer() LPsway scale {scale100}";
			return OS();
		}
	}
}
