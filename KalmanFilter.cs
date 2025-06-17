using SimHub.Plugins.OutputPlugins.Dash.GLCDTemplating;
using System;

public class KalmanFilter
{
    private readonly double Q = 0.125;

	public KalmanFilter()		// linear quadratic estimation
	{
	}

	public double[] Init()
	{
		return new double[] {	0,		// filtered input
								1 };	// uncertainty
	}

	public double Filter(double input, ref double [] k)
	{
		return Filter(input, Q, ref k);
	}

	public double Filter(double input, double noise, ref double [] k)
	{
		double filtered = k[0], uncertainty = k[1];

		uncertainty += noise;			// update prediction uncertainty
		// uncertainty monotonically decreases to limit determined by noise
		uncertainty /= (uncertainty + 1);
		k[1] = uncertainty;

		double d = input - filtered;
		// weight change by uncertainty
        return k[0] = filtered + uncertainty * d;
	}
}
