namespace blekenbleu.loaded
{
	/// <summary>
	/// Settings class, make sure it can be correctly serialized using JSON.net
	/// </summary>
	public class Settings
	{
		public int Gain = 100, YawScale = 25, RRscale = 25, SwayScale = 45;
		public short Thresh_sv = 3, Thresh_sh = 15, Thresh_ss = 15, SteerFact = 75;
		public short Filter_L = 15;
	}
}
