using SimHub.Plugins;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		string GameDBText, LoadStr, DeflStr, oops = "";
		double LoadFL, LoadFR, LoadRL, LoadRR, DeflFL, DeflFR, DeflRL, DeflRR;
		double LPyaw = 0, LPsway = 0;
		double ACprodFRslip = 0;
		double Loads, Heave, YawRate, Steering, SteerPC, Vsway;
		double SurgeAcc = 0, Roll = 0, DRoll = 0, Pitch = 0, DPitch = 0;

		void Attach()
		{
			// Declare properties available in the property list
			// these get evaluated "on demand" (when shown or used in formulas)
			this.AttachDelegate("Gain",			() => Settings.Gain);
			this.AttachDelegate("Game",			() => GameDBText);
			this.AttachDelegate("Heave",		() => $"{Heave:0.000}");
			this.AttachDelegate("Speed, Surge", () => $"{SpeedKmh:#0.0}, {SurgeAcc:0.000}");
			this.AttachDelegate("Thresh_sh",	() => 0.01 * View.Model.Thresh_sh);
			this.AttachDelegate("Thresh_ss",	() => 0.01 * View.Model.Thresh_ss);
			this.AttachDelegate("Thresh_sv",	() => View.Model.Thresh_sv);
			this.AttachDelegate("DRoll",		() => DRoll);
			this.AttachDelegate("DPitch",		() => DPitch);
			this.AttachDelegate("OverSteer",	() => OverSteer()); // attitude - trajectory: ayaw - View.Model.YawScale * asway
			this.AttachDelegate("SpeedKmh",		() => Paused ? 0 : Kalman.Filter(SpeedKmh, ref Kkmh));	// Kalman-filtered SpeedKmh
			this.AttachDelegate("KSwayAcc",		() => Paused ? 0 : Kalman.Filter(SwayAcc, 0.8, ref Kswa));
			this.AttachDelegate("SwayRate",		() => SwayRate);	// 1000 * SwayAcc / SpeedKmh
			this.AttachDelegate("SwayRate%",	() => 0.0025 * SwayRate);	// 1000 * SwayAcc / SpeedKmh
			this.AttachDelegate("SwayVsteer",	() => SwayVsteer);
			this.AttachDelegate("Vsway",		() => Vsway);		// game dependent
			this.AttachDelegate("YawRate",		() => YawRate);		// OrientationYawVelocity radians per second
			this.AttachDelegate("YawRate%",		() => 0.01 * YawRate);		// OrientationYawVelocity radians per second
			this.AttachDelegate("YawVsteer",	() => YawVsteer);
			this.AttachDelegate("KYawRate",		() => Paused ? 0 : Kalman.Filter(YawRate, ref Kyaw));
			this.AttachDelegate("YawSway",		() => YawSway);

			/// RangeyRover properties
			this.AttachDelegate("RangeyRover",	() => RangeyRover());// rear - front slip angle including steering
			this.AttachDelegate("Steering",		() => Steering);	// game-dependent steering angle
			this.AttachDelegate("Steer%",		() => -SteerPC);		// game-dependent steering percent
			this.AttachDelegate("RRyaw_rate",	() => yaw_rate);	// usually OrientationYawVelocity
			this.AttachDelegate("RRVlateral",	() => Vlateral);	// usually AccelerationSway
			this.AttachDelegate("RRVlong",		() => Vlong);		// usually SpeedKmh
			this.AttachDelegate("RRSwayRadians",() => SwayRadians);
			this.AttachDelegate("RRSwayRatio",  () => SwayRatio);
			this.AttachDelegate("RRSwayScale",	() => RRSwayScale);
			this.AttachDelegate("RRfront_slip", () => front_slip_angle);
			this.AttachDelegate("RRYawRadians",	() => YawRadians);
			this.AttachDelegate("RRyawSway",	() => RRyawSway);
			
			this.AttachDelegate("oops",			() => oops);
			if (GameDBText == "AssettoCorsa")
				this.AttachDelegate("ACprodFRslip",() => ACprodFRslip);

			if (null != DeflStr)
			{
				this.AttachDelegate("FLdefl",	() => DeflFL);
				this.AttachDelegate("FRdefl",	() => DeflFR);
				this.AttachDelegate("RLdefl",	() => DeflRL);
				this.AttachDelegate("RRdefl",	() => DeflRR);
				this.AttachDelegate("Defl",		() => (DeflFL + DeflFR + DeflRL + DeflRR) / 4);
				this.AttachDelegate("DeflPitch",() => (DeflFL + DeflFR - (DeflRL + DeflRR)));
				this.AttachDelegate("DeflRollF",() => (DeflFL - DeflFR));
				this.AttachDelegate("DeflRollR",() => (DeflRL - DeflRR));
				this.AttachDelegate("DeflHeaveR",() => (DeflRL + DeflRR) / 2);
				this.AttachDelegate("DeflHeaveF",() => (DeflFL + DeflFR) / 2);
			}
			if (null != LoadStr)
			{
				this.AttachDelegate("FRload",	() => LoadFR);
				this.AttachDelegate("FLload",	() => LoadFL);
				this.AttachDelegate("RRload",	() => LoadRR);
				this.AttachDelegate("RLload",	() => LoadRL);
				this.AttachDelegate("Load",		() => Loads);
			}
		}
	}
}
