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
		string[] corner, dorner;
		public string PluginVersion = FileVersionInfo.GetVersionInfo(
			Assembly.GetExecutingAssembly().Location).FileVersion.ToString(); 
		public Settings Settings;

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

		PluginManager pm;

		double Prop(string parm)
		{
			var value = pm.GetPropertyValue(parm);
			if (null == value)
			{
				oops = $"Prop({parm}):  null value";
				return 0;
			}
			return Convert.ToDouble(value);
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
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			// Define the value of our property (declared in init)
			if (!data.GameRunning || null == data.OldData || null == data.NewData)
				return;
			try
			{
				pm = pluginManager;
				Heave = data.NewData.AccelerationHeave ?? 0;
				LSpeed = data.NewData.SpeedLocal;
				LSurge = data.NewData.AccelerationSurge ?? 0;
				var old = Pitch;
				Pitch = data.NewData.OrientationPitch;
                DPitch = Pitch - old;
				old = Roll;
				Roll = data.NewData.OrientationRoll;
				DRoll = Roll - old;
                SpeedKmh = data.NewData.SpeedKmh;
				SwayAcc = data.NewData.AccelerationSway ?? 0;
				SwayRate = (5 < SpeedKmh) ? SwayAcc / SpeedKmh : 0;
				YawVel = View.Model.YawVelGain * 0.01 * data.NewData.OrientationYawVelocity;

				// game-specific properties
				Steering = Prop(Psteer);
				// Local velocities in metres per second
				Vsway = Prop(Psway);
				YawRate = Prop(Pyaw);  // Angular velocities in radians per second
				Load();
				if (GameDBText == "AssettoCorsa")
					ACprodFRslip = Slip('4') * Slip('3') - Slip('2') * Slip('1');
			} catch (Exception e)
			{
                string oops = e?.ToString();
                SimHub.Logging.Current.Info("DataUpdate() Failed: " + oops);
            }
        }

		/// <summary>
		/// Called at plugin manager stop, close/dispose anything needed here !
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void End(PluginManager pluginManager)
		{
			// Save settings
			Settings.YawVelGain = View.Model.YawVelGain;
			Settings.Thresh_sv = View.Model.Thresh_sv;
			Settings.Thresh_sh = View.Model.Thresh_sh;
			Settings.Thresh_ss = View.Model.Thresh_ss;
			Settings.Filter_L = View.Model.Filter_L;
			this.SaveCommonSettings("GeneralSettings", Settings);
		}

		/// <summary>
		/// Returns the settings control, return null if no settings control is required
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <returns></returns>
		public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return View = new Control(this);
		}

		/// <summary>
		/// Called once after plugins startup
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void Init(PluginManager pluginManager)
		{
			SimHub.Logging.Current.Info("Starting " + LeftMenuTitle);
			Game(GameDBText = pluginManager.GameName);

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
				SimHub.Logging.Current.Info(LeftMenuTitle + $"Gain {Settings.Gain}");
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
		}
	}
}
