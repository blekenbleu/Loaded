using GameReaderCommon;
using SimHub.Plugins;
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
		string GameDBText, LoadStr, DeflStr, CarId = "";
		double LoadFL, LoadFR, LoadRL, LoadRR, DeflFL, DeflFR, DeflRL, DeflRR;
        readonly double[] Defl0 = new double[] { 0, 0 };
        readonly double[] Defl0Avg = new double[] { 0, 0 };
        double Heave;
		uint Zero = 0;
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
		public string LeftMenuTitle => "Loaded";

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

			Heave = (double)data.NewData.AccelerationHeave;
			Load(pluginManager, ref data);
			/* Capture suspension travel values when speed and heave are 0
			 ; changes from those values should correlate to changes in load
			 ; changes from average (total) should correlate to heave
			 ; rescale average suspension travel by heave to normalize loads
			 ; front and rear suspension spring rates typically differ...
			 */

			if (data.OldData.SpeedKmh < Settings.Gain && data.OldData.SpeedKmh >= Settings.Gain)
			{
				// Trigger an event
				this.TriggerEvent("GainChange");
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
			SimHub.Logging.Current.Info("Starting plugin version " + PluginVersion);
			Game(GameDBText = pluginManager.GameName);

            // Declare properties available in the property list
            // these get evaluated "on demand" (when shown or used in formulas)
            this.AttachDelegate("Game", () => GameDBText);
            this.AttachDelegate("Heave", () => Heave);
			this.AttachDelegate("Thresh_sh", () => 0.01 * View.Model.Thresh_sh);
			this.AttachDelegate("Thresh_ss", () => 0.01 * View.Model.Thresh_ss);
			this.AttachDelegate("Thresh_sv", () => View.Model.Thresh_sv);
			if (null != DeflStr)
			{
				this.AttachDelegate("Deflections", () =>
					$"{DeflFR:0.0000}, {DeflFL:0.0000}, {DeflRR:0.0000}, {DeflRL:0.0000}");
				this.AttachDelegate("Defl0Avg", () => $"{Defl0Avg[0]:##0.000}, {Defl0Avg[1]:##0.000}");
				this.AttachDelegate("Defl0Count", () => Zero);
			}
			if (null != LoadStr)
				this.AttachDelegate("Loads", () =>
                  $"{LoadFR:####0.0000}, {LoadFL:####0.0000}, {LoadRR:####0.0000}, {LoadRL:####0.0000}");

			// Load settings
			Settings = this.ReadCommonSettings<Settings>("GeneralSettings", () => new Settings());

			this.AttachDelegate("Gain", () => Settings.Gain);

			// Declare an event
			this.AddEvent("GainChange");

			// Declare an action which can be called
			this.AddAction("IncrementGain",(a, b) =>
			{
				Settings.Gain += Settings.Gain >> 3;
				SimHub.Logging.Current.Info(LeftMenuTitle + $"Gain {Settings.Gain}");
			});

			// Declare an action which can be called
			this.AddAction("DecrementGain", (a, b) =>
			{
				Settings.Gain -= Settings.Gain >> 3;
				if (10 > Settings.Gain)
					Settings.Gain = 10;
				SimHub.Logging.Current.Info(LeftMenuTitle + $"Gain {Settings.Gain}");
			});
		}
	}
}
