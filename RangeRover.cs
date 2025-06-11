using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace blekenbleu.loaded
{
	public partial class Loaded // : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		PluginManager pm;

		double Prop(string parm)
		{
			string data = "DataCorePlugin." + parm;
			var value = pm.GetPropertyValue(data);
			return (null == value) ? 0 : Convert.ToDouble(value);
		}

		// Automobilista 2
		double RangeRover(PluginManager pluginManager, ref GameData data)
		{
			pm = pluginManager;

			// Angular velocities in radians per second
			var yaw_rate = Prop("GameRawData.mAngularVelocity02");

			// Assumed distances to front and rear axles in meters unavailable in game
			var Df = 1.2;			// Distance to front axle
			var Dr = 1.6;			// Distance to rear axle

			// Typical steering angle 24 degrees, match to what is set in game
			double steeringRad = 24 * Math.PI/180,
				  Rd = 180/Math.PI;
			// Normalized steering input [ RANGE = -1.0f->1.0f ]
			var steeringAngleRad = steeringRad * Prop("GameRawData.mSteering");

			// Local velocities in metres per second
			var Vlat = Prop("GameRawData.mLocalVelocity01");
			var Vlong = Prop("GameRawData.mLocalVelocity03");

			// slip angles by simplified equation
			var front_slip_angle = steeringAngleRad - Math.Atan((Vlat + Df * yaw_rate) / Vlong);
			var rear_slip_angle = -Math.Atan((Vlat - Dr * yaw_rate) / Vlong);

			// Oversteer in degrees
			return Rd * (rear_slip_angle - front_slip_angle);	// positive = Oversteer
		}
	}
}
