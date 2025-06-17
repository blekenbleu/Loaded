using SimHub.Plugins;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		string GameDBText, LoadStr, DeflStr, oops = "";
		double LoadFL, LoadFR, LoadRL, LoadRR, DeflFL, DeflFR, DeflRL, DeflRR;
		double LPdiff = 0, LPyaw = 0, LPsway = 0;
		double ACprodFRslip = 0;
		double Loads, Heave, YawRate, Steering, Vsway, KSwayAcc, KYawRate;
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
			this.AttachDelegate("LPdiff",		() => LPdiff);		// OverSteer() LPdiff = LPyaw - View.Model.OverSteerGain * LPsway
			this.AttachDelegate("RangeRover",	() => RangeRover());// slip angle including steering
			this.AttachDelegate("OverSteer",	() => OverSteer()); // ayaw - View.Model.OverSteerGain * asway
			this.AttachDelegate("Steering",		() => Steering);	// game-dependent steering angle
			this.AttachDelegate("SpeedKmh",		() => SpeedKmh);	// Kalman-filtered SpeedKmh
			this.AttachDelegate("KSwayAcc",		() => KSwayAcc);	// Kalman-filtered Sway Acceleration
			this.AttachDelegate("SwayRate",		() => SwayRate);	// 1000 * SwayAcc / SpeedKmh
			this.AttachDelegate("Vsway",		() => Vsway);		// game dependent
			this.AttachDelegate("YawRate",		() => YawRate);		// OrientationYawVelocity radians per second
			this.AttachDelegate("KYawRate",		() => KYawRate);	// Kalman-filtered OrientationYawVelocity radians per second
			
			this.AttachDelegate("oops",			() => oops);
			if (GameDBText == "AssettoCorsa")
				this.AttachDelegate("ACprodFRslip",	() => ACprodFRslip);
			if (null != DeflStr)
			{
				this.AttachDelegate("FLdefl",	() => DeflFL);
				this.AttachDelegate("FRdefl",	() => DeflFR);
				this.AttachDelegate("RLdefl",	() => DeflRL);
				this.AttachDelegate("RRdefl",	() => DeflRR);
				this.AttachDelegate("Defl",		() => (DeflFL + DeflFR + DeflRL + DeflRR) / 4);
				this.AttachDelegate("DeflPitch", () => (DeflFL + DeflFR - (DeflRL + DeflRR)));
				this.AttachDelegate("DeflRollF", () => (DeflFL - DeflFR));
				this.AttachDelegate("DeflRollR", () => (DeflRL - DeflRR));
				this.AttachDelegate("DeflHeaveR", () => (DeflRL + DeflRR) / 2);
				this.AttachDelegate("DeflHeaveF", () => (DeflFL + DeflFR) / 2);
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
