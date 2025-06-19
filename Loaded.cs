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
			Settings.Filter_L  = View.Model.Filter_L;
			Settings.MatchGain = View.Model.RRfactor;
			Settings.SlipGain  = View.Model.YawScale;
			Settings.SwayGain  = View.Model.SwayScale;
			Settings.Thresh_sv = View.Model.Thresh_sv;
			Settings.Thresh_sh = View.Model.Thresh_sh;
			Settings.Thresh_ss = View.Model.Thresh_ss;
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
		}

		/// <summary>
		/// Returns the settings control, return null if no settings control is required
		/// Must get called after Init(), since it initializes View.Model values to Settings
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <returns></returns>
		public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return View = new Control(this, PluginVersion);
		}

		PluginManager pm;

		double Prop(string parm)
		{
			if (0 == parm.Length)
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
			// Update property values (declared in Init)
			if (!data.GameRunning || null == data.OldData || null == data.NewData || null == data.NewData.CarId)
				return;

            try
			{
				pm = pluginManager;
                // skip LPfilter() if not updating
                Paused = 0 == (SpeedKmh = data.NewData.SpeedKmh);
                PacketTime = data.NewData.CurrentLapTime;
				Heave = data.NewData.AccelerationHeave ?? 0;
				SurgeAcc = data.NewData.AccelerationSurge ?? 0;

				var old = Pitch;								// potentially for load estimates
				Pitch = data.NewData.OrientationPitch;
                DPitch = Pitch - old;
				old = Roll;
				Roll = data.NewData.OrientationRoll;
				DRoll = Roll - old;

                // Local velocities and acceleration
				SwayAcc = data.NewData.AccelerationSway ?? 0;
				// radians per second?
				YawRate = 50 < data.NewData.OrientationYawVelocity ? 50 : data.NewData.OrientationYawVelocity;
				SwayRate = (5 < SpeedKmh) ? 1000 * SwayAcc / SpeedKmh : 0;

				// game specific properties
				// Normalized steering input:   [ -1.0f <= Prop(Psteer) <= 1.0f ]
				Steering = stang * Prop(Psteer);	// +/- 1 to -/+ degrees
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
