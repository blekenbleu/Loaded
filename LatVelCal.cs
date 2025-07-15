using MathNet.Numerics;

namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		readonly double[] ss = new double[900], ls = new double[900];

		/* accumulate LatAcc and SwayAcc sample
		 ; least-squares fit straight line
		 ; apply slope to LatAcc to scale for SlipRate
		 */
		void LatVelCal(double latAcc)
		{
			double s = SwayAcc / ss[View.Model.LAi-1], l = latAcc / ls[View.Model.LAi-1];

			if (s > 1.02 || s < 0.98 || l > 1.02 || l < 0.98)
			{
				ss[View.Model.LAi] = SwayAcc;
				ls[View.Model.LAi++] = latAcc;
				if (ss.Length == View.Model.LAi)
				{
					(double, double)fl = Fit.Line(ls, ss);
					View.Model.LAscale = fl.Item2;
				}
			}
		}
	}
}
