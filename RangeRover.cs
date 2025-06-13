using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace blekenbleu.loaded
{
	public partial class Loaded // : IPlugin, IDataPlugin, IWPFSettingsV2
	{
        // set property names in Game(); values in DataUpdate()
		// Call in AttachDelegate()
        double RangeRover()
		{
			// Assumed distances to front and rear axles in meters unavailable in game
			var Df = 1.2;			// Distance to front axle
			var Dr = 1.6;			// Distance to rear axle

			// Typical steering angle 24 degrees, match to what is set in game
			double steeringRad = 24 * Math.PI/180, Rd = 180/Math.PI;

			// slip angles by simplified equation
			if (5 > SpeedKmh)
				return 0;

			// Normalized steering input [ RANGE = -1.0f->1.0f ]
			var steeringAngleRad = steeringRad * Steering;
			var front_slip_angle = steeringAngleRad - Math.Atan((Vsway + Df * YawRate) / SpeedKmh);
			var rear_slip_angle = -Math.Atan((Vsway - Dr * YawRate) / SpeedKmh);

			// Oversteer in degrees
			return Rd * (rear_slip_angle - front_slip_angle);	// positive = Oversteer
		}
	}
}
