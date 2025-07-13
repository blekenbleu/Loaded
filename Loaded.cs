using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;

namespace blekenbleu.loaded
{
	[PluginDescription("estimate wheel loads similar to Assetto Corsa")]
	[PluginAuthor("blekenbleu")]
	[PluginName("Loaded")]
	public partial class Loaded : IPlugin, IDataPlugin, IWPFSettingsV2
	{
		Control View;
		public string PluginVersion = FileVersionInfo.GetVersionInfo(
			Assembly.GetExecutingAssembly().Location).FileVersion.ToString(); 
		public Settings Settings;
		KalmanFilter Kalman;
		double[] Kyaw, Kswa, Kkmh;
		// Typical steering angle 24 degrees; set to game's car value
		readonly float stang = -24;			// change sign to match OrientationYawVelocity, AccelerationSway 
		readonly double Rd = 180 / Math.PI;
		ushort Srun;


		/// <summary>
		/// Instance of the current plugin manager
		/// </summary>
		public PluginManager PluginManager { get; set; }

		/// <summary>
		/// Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
		/// </summary>
		public ImageSource PictureIcon => this.ToIcon(Properties.Resources.LoadedIcon);

		/// <summary>
		/// Gets a short plugin title to show in left menu.
		/// Return null if you want to use the title as defined in PluginName attribute.
		/// </summary>
		public string LeftMenuTitle => "Loaded " + PluginVersion;

		/// <summary>
		/// Called at plugin manager stop, close/dispose anything needed here !
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void End(PluginManager pluginManager)
		{
			// Save settings
			this.SaveCommonSettings("GeneralSettings", Settings);
		}

		/// <summary>
		/// Called once after plugins startup
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void Init(PluginManager pluginManager)
		{
			SimHub.Logging.Current.Info("Starting " + LeftMenuTitle);
			Game(GameDBText = pluginManager.GameName, "DataCorePlugin.GameRawData.");

			// Load settings
			Settings = this.ReadCommonSettings<Settings>("GeneralSettings", () => new Settings());
			Attach();

			// Declare an event
			this.AddEvent("GainChange");

			// Declare an action which can be called
			this.AddAction("IncrementGain",(a, b) =>
			{
				int bump = Settings.Gain >> 3;

				if (1 > bump)
					bump = 1;
				Settings.Gain += bump;
				View.Dispatcher.Invoke((Action)(() =>
					{ View.gl.Title = $"Load gain = {Settings.Gain:##0.00}"; }
				));
			});

			// Declare an action which can be called
			this.AddAction("DecrementGain", (a, b) =>
			{
				int bump = Settings.Gain >> 3;

				if (1 > bump)
					bump = 1;
				Settings.Gain -= bump;
				if (1 > Settings.Gain)
					Settings.Gain = 1;
				View.Dispatcher.Invoke((Action)(() =>
					{ View.gl.Title = $"Load gain = {Settings.Gain:##0.00}"; }
				));
			});
			Kalman = new KalmanFilter();
			Kyaw = Kalman.Init();
			Kswa = Kalman.Init();
			Kkmh = Kalman.Init();
			Srun = 0;		// try to detect OverSteer() or RangeyRover()
		}

		/// <summary>
		/// Returns the settings control, return null if no settings control is required
		/// Must get called after Init(), since it initializes View.Model values to Settings
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <returns></returns>
		public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return View = new Control(this);
		}

		PluginManager pm;

		double Prop(string parm)
		{
			if (0 == parm.Length || null == pm)
				return 0;

			var value = pm.GetPropertyValue(parm);
			if (null == value)
			{
				oops = $"Prop({parm}):  null value";
				return 0;
			}
			return Convert.ToDouble(value);
		}

		double Slip(char wheel)
		{
			string raw = "DataCorePlugin.GameRawData.Physics.WheelSlip0" + wheel;
			var slip = pm.GetPropertyValue(raw);
			return (null == slip) ? 0 : Convert.ToDouble(slip);
		}

		/// <summary>
		/// Called one time per game data update, contains all normalized game data,
		/// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
		///
		/// This method is on the critical path
		/// it must execute as fast as possible and avoid throwing any error
		///
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including current and previous data frame.</param>
		TimeSpan PacketTime = new TimeSpan();
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			pm = pluginManager;

			// Update property values (declared in Init)
			if (!data.GameRunning || null == data.OldData
			 || null == data.NewData || null == data.NewData.CarId)
			{
				Paused = true;
				Heave = SurgeAcc = SwayAcc = SwayRate = SlipRate = YawVel = LatAcc = 0;
				return;
			}

			try
			{
				if (View.Model.Recal)
				{
					Settings.Gtot = Settings.Stot = Settings.scaleTot = Settings.Gct
								  = Settings.scaleCt = Settings.Sct = 0;
					View.Model.Recal = false;
				}
				if (10 < Srun)	// OverSteer(), RangeyRover() heartbeat
				{
					Srun = 0;
					if ("Red" == View.Model.ModeColor)
						View.Model.ModeColor = "Green";
				} else Srun++;

				// skip LPfilter() if not updating
				PacketTime = data.NewData.CurrentLapTime;
				double abSteer = Math.Abs(Prop(Psteer));
				if (abSteer > View.Model.Stlo && 5 < data.NewData.SpeedKmh)
				{
					SpeedKmh = data.NewData.SpeedKmh;
					// Normalized steering input:   [ -1.0f <= Prop(Psteer) <= 1.0f ]
					SteerPC = Prop(Psteer);
					Paused = false;
					Heave = data.NewData.AccelerationHeave ?? 0;
					SurgeAcc = data.NewData.AccelerationSurge ?? 0;
					// Local velocities and acceleration
					SwayAcc = data.NewData.AccelerationSway ?? 0;
					SwayRate = 1000 * SwayAcc / SpeedKmh;
					// radians per second?
					YawVel = data.NewData.OrientationYawVelocity;
					if (50 < YawVel)
						YawVel = 50;
					LatAcc = 0.0055 * YawVel * SpeedKmh;		// ideal lateral acceleration for current Yaw Velocity
					SlipRate = LatAcc - SwayAcc;
					SlipAngleRate = YawVel - 0.2 * SwayRate;
				}
				else Paused = true;

				var old = Pitch;								// potentially for load estimates
				Pitch = data.NewData.OrientationPitch;
				DPitch = Pitch - old;
				old = Roll;
				Roll = data.NewData.OrientationRoll;
				DRoll = Roll - old;

				// game specific properties
				Steering = stang * SteerPC;	// +/- 1 to -/+ degrees
				Vsway = Prop(Psway);
				Load();
				if (GameDBText == "AssettoCorsa")
					ACprodFRslip = Slip('4') * Slip('3') - Slip('2') * Slip('1');
			} catch (Exception e)
			{
				string oops = e?.ToString();
				SimHub.Logging.Current.Info("DataUpdate() Failed: " + oops);
			}
		}	// DataUpdate()
	}
}
