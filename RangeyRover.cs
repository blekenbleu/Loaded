using System;
using System.Windows;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		double MRyaw = 0, MRsway = 0, SwayRadians, YawRadians;
		double yaw_rate, Vlateral, Vlong, RRSwayScale, RRscale;
		double RRyawSway, front_slip_angle, SwayRatio;
		double scaleTot = 0;
		ushort scaleCt = 0;
/*
 ;		The concept:  for low sway, yaw and trajectory should be similar and linearly related;
 ;		oversteer and understeer should come in with higher cornering loads and steering inputs.
 ;		Establish a scale that minimizes yaw and sway rate differences for low cornering loads.
 */
		double MatchRates() // multiplied by SwayRadians
		{
			RRscale = 0.001 * View.Model.RRscale;
			if (Paused || 20000 < scaleCt || 0D == SwayRadians || Math.Sign(YawRadians) != Math.Sign(SwayRadians))
				return RRscale;

			double ayaw = Math.Abs(YawRadians);
			double asway = Math.Abs(SwayRadians);
			if (ayaw > 0.01D || asway > .1D)
				return RRscale;

			LPfilter(ref MRyaw, 10, 40000 * ayaw);
			LPfilter(ref MRsway, 10, 40000 * asway);
			RRscale = 1000 * MRyaw / MRsway;	// unity scale is 100 on slider
			scaleCt++;
			scaleTot += (1 > RRscale) ? 1 : 190 < RRscale ? 190 : RRscale;
			View.Model.RRscale = (int)(0.5 + scaleTot / scaleCt);
			return 0.001 * View.Model.RRscale;
		}

		// AttachDelegate() calls this
		// property names set in Game(); values in DataUpdate()
		double RangeyRover()
		{	// slip angles by simplified equation, ignoring CoG
			Srun = 0;
			if (19999 > scaleCt && Visibility.Visible == View.Model.ButtonVisibility
				&& "Green" == View.Model.ModeColor)
				View.Model.ModeColor = "Red";

			Vlong = (GameDBText != "Automobilista2") ? SpeedKmh / 3:
									- Prop("DataCorePlugin.GameRawData.mLocalVelocity03");

			if (2 > Vlong)
				return 0;		// not only don't care, calculations blow up near 0

			if (GameDBText == "Automobilista2")
			{
				yaw_rate = Prop("GameRawData.mAngularVelocity02");
				Vlateral = - Prop("DataCorePlugin.GameRawData.mLocalVelocity01");
			}
			else
			{
				yaw_rate = YawRate / 57;
				Vlateral = 0.364 * SwayAcc;
			}
			YawRadians = yaw_rate / Vlong;

			SwayRadians = Math.Atan(SwayRatio = Vlateral / Vlong);
			// match SwayRadians and YawRadians for low slips
			RRSwayScale = SwayRadians * MatchRates();

			RRyawSway = Rd * (YawRadians - RRSwayScale);	// orientation - trajectory
			// front_slip_angle is difference between direction wheels point and trajectory
			// vehicle yaw adds to wheel direction
			front_slip_angle = Steering + RRyawSway;

			// Oversteer in degrees
			return Math.Abs(RRyawSway) - Math.Abs(front_slip_angle);	// positive = Oversteer
		}
	}
}
