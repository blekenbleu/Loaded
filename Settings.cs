using System.Windows;

namespace blekenbleu.loaded
{
	/// <summary>
	/// Settings class, make sure it can be correctly serialized using JSON.net
	/// </summary>
	public class Settings
	{
		public int Gain = 100, swayScale = 20, yawScale = 20, rrScale = 20;
        public short steerFact = 30, filter_L = 15, thresh_ss = 15, thresh_sh = 15, thresh_sv = 3;
        public ushort Gct = 0, Sct = 0, scaleCt = 0;
		public double Gtot = 0, Stot = 0, scaleTot = 0, Stlo = 0.15, Sthi = 2.5, LAscale = 0.00485;
		public bool recal = false;
		public string mode = "", color = "Green";
		public Visibility svis = Visibility.Hidden;
	}
}
