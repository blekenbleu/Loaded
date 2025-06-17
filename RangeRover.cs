using System;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		double MRyaw = 0, MRsway = 0, SwayRadians, YawRadians;
		double yaw_rate, Vlateral, Vlong, SwayScaled;
		double rear_slip_angle, front_slip_angle;
		double MatchRates()
		{
			if (1 < SwayAcc || -1 > SwayAcc)
				return View.Model.MatchGain;

			double ayaw = Math.Abs(YawRate);
			double asway = Math.Abs(SwayRadians);
			if (ayaw > 20 || asway > 1)
				return View.Model.MatchGain;

			LPfilter(ref MRyaw, 10, YawRate);
			LPfilter(ref MRsway, 10, SwayAcc);
			var diff = MRyaw - MRsway;
			if (1 > diff && -1 < diff)
			{
				double gain = MRyaw / MRsway;
				if (10 < gain && 90 > gain)
					View.Model.MatchGain = (int)(0.5 * gain);
			}
			return View.Model.MatchGain;
		}

		// AttachDelegate() calls this
		// property names set in Game(); values in DataUpdate()
		// Typical steering angle 24 degrees, match to what is set in game
		readonly double steeringRad = 24 * Math.PI/180, Rd = 180/Math.PI;
		double RangeRover()
		{   // slip angles by simplified equation,  ignoring CoG
			Vlong = (GameDBText != "Automobilista2") ? SpeedKmh :
							Prop("GameRawData.mLocalVelocity03");

            if (5 > Vlong)
				return 0;       // not only don't care, calculations blow up near 0

			if (GameDBText == "Automobilista2")
			{
				yaw_rate = Prop("GameRawData.mAngularVelocity02");
				Vlateral = Prop("GameRawData.mLocalVelocity01");
			}
			else
			{
				yaw_rate = YawRate;
				Vlateral = SwayAcc;
            }
			YawRadians = yaw_rate / Vlong;

            SwayRadians = Math.Atan(Vlateral / Vlong);
            // match SwayRadians and YawRate for low slips
            SwayScaled = SwayRadians * MatchRates();

			rear_slip_angle = Rd * (YawRadians - SwayScaled);	// orientation - trajectory
			// Normalized input [ -1.0f <= Steering <= 1.0f ]
			var steeringAngle = steeringRad * Steering;
			// front_slip_angle is difference between direction wheels point and trajectory
			// vehicle yaw velocity adds to wheel direction
			front_slip_angle = Rd * ((steeringAngle + YawRadians) - SwayScaled);

			// Oversteer in degrees
			return Math.Abs(rear_slip_angle) - Math.Abs(front_slip_angle);	// positive = Oversteer
		}
	}
}
