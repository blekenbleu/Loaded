namespace blekenbleu.loaded
{
	/// <summary>
	/// Settings class, make sure it can be correctly serialized using JSON.net
	/// </summary>
	public class Settings
	{
		public int Gain = 100, SlipGain = 25, MatchGain = 25, SwayGain = 45;
		public short Thresh_sv = 3, Thresh_sh = 15, Thresh_ss = 15;
		public short Filter_L = 15;
	}
}
