using System.Windows;

namespace blekenbleu.loaded
{
	/// <summary>
	/// Settings class, make sure it can be correctly serialized using JSON.net
	/// </summary>
	public class Settings
	{
		public int Gain = 100, YawScale = 25, RRscale = 25, SwayScale = 45;
		public short Thresh_sv = 3, Thresh_sh = 15, Thresh_ss = 15, SteerFact = 30;
		public short Filter_L = 15;
		public ushort Gct = 0, Sct = 0, scaleCt = 0;
		public double Gtot = 0, Stot = 0, scaleTot = 0;
		public bool recal = false;
		public string mode = "", color = "Green";
		public Visibility svis = Visibility.Hidden;
		public int swayScale = 20, yawScale = 20, rrScale = 20;
		public short steerFact = 70, filter_L = 15, thresh_ss = 15, thresh_sh = 15, thresh_sv = 3;
	}
}
