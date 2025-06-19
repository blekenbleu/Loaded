using System;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		double MRyaw = 0, MRsway = 0, SwayRadians, YawRadians;
		double yaw_rate, Vlateral, Vlong, RRSwayScale, RRgain;
		double RRyawSway, front_slip_angle, SwayRatio;
		double gainTot = 0;
		ushort gainCt = 0;
/*
 ;		The concept:  for low sway, yaw and trajectory should be similar and linearly related;
 ;		oversteer and understeer should come in with higher cornering loads and steering inputs.
 ;		Establish a gain that minimizes yaw and sway rate differences for low cornering loads.
 */
		double MatchRates() // multiplied by SwayRadians
		{
			RRgain = 0.001 * View.Model.RRfactor;
			if (Paused || 20000 < gainCt || 0 == SwayRadians || 0 > YawRadians / SwayRadians)
				return RRgain;

			double ayaw = Math.Abs(YawRadians);
			double asway = Math.Abs(SwayRadians);
			if (ayaw > 0.01 || asway > .1)
				return RRgain;

			LPfilter(ref MRyaw, 10, 40000 * ayaw);
			LPfilter(ref MRsway, 10, 40000 * asway);
			RRgain = 1000 * MRyaw / MRsway;	// unity gain is 100 on slider
			gainCt++;
			gainTot += (1 > RRgain) ? 1 : 190 < RRgain ? 190 : RRgain;
			View.Model.RRfactor = (int)(0.5 + gainTot / gainCt);
			return 0.001 * View.Model.RRfactor;
		}

		// AttachDelegate() calls this
		// property names set in Game(); values in DataUpdate()
		double RangeyRover()
		{   // slip angles by simplified equation,  ignoring CoG
			Vlong = (GameDBText != "Automobilista2") ? SpeedKmh / 3:
									- Prop("DataCorePlugin.GameRawData.mLocalVelocity03");

			if (2 > Vlong)
				return 0;	   // not only don't care, calculations blow up near 0

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
