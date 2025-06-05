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
		string GameDBText, LoadStr, DeflStr, CarId = "";
		double LoadFL, LoadFR, LoadRL, LoadRR, DeflFL, DeflFR, DeflRL, DeflRR;
		double Defl, Loads;
		double Heave, LSpeed, LSurge = 0, Roll = 0, DRoll = 0, Pitch = 0, DPitch = 0;
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
			LSpeed = data.NewData.SpeedLocal;
			LSurge = (double)data.NewData.AccelerationSurge;
			DRoll = Roll;
			Roll = (double)data.NewData.OrientationRoll;
			DRoll -= Roll;
			DPitch = Pitch;
			Pitch = (double)data.NewData.OrientationPitch;
			DPitch =- Pitch;
			Load(pluginManager, ref data);
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
			SimHub.Logging.Current.Info("Starting " + LeftMenuTitle);
			Game(GameDBText = pluginManager.GameName);

			// Declare properties available in the property list
			// these get evaluated "on demand" (when shown or used in formulas)
			this.AttachDelegate("Game", () => GameDBText);
			this.AttachDelegate("Heave", () => $"{Heave:0.000}");
			this.AttachDelegate("Speed, Surge", () => $"{LSpeed:#0.0}, {LSurge:0.000}");
			this.AttachDelegate("Thresh_sh", () => 0.01 * View.Model.Thresh_sh);
			this.AttachDelegate("Thresh_ss", () => 0.01 * View.Model.Thresh_ss);
			this.AttachDelegate("Thresh_sv", () => View.Model.Thresh_sv);
			this.AttachDelegate("DRoll", () => DRoll);
			this.AttachDelegate("DPitch", () => DPitch);
			if (null != DeflStr)
			{
				this.AttachDelegate("FLdefl", () => DeflFL);
				this.AttachDelegate("FRdefl", () => DeflFR);
				this.AttachDelegate("RLdefl", () => DeflRL);
				this.AttachDelegate("RRdefl", () => DeflRR);
				this.AttachDelegate("Defl", () => (DeflFL + DeflFR + DeflRL + DeflRR) / 4);
				this.AttachDelegate("DeflPitch", () => (DeflFL + DeflFR - (DeflRL + DeflRR)));
				this.AttachDelegate("DeflRollF", () => (DeflFL - DeflFR));
				this.AttachDelegate("DeflRollR", () => (DeflRL - DeflRR));
				this.AttachDelegate("DeflHeaveR", () => (DeflRL + DeflRR) / 2);
				this.AttachDelegate("DeflHeaveF", () => (DeflFL + DeflFR) / 2);
			}
			if (null != LoadStr)
			{
				this.AttachDelegate("FRload", () => LoadFR);
				this.AttachDelegate("FLload", () => LoadFL);
				this.AttachDelegate("RRload", () => LoadRR);
				this.AttachDelegate("RLload", () => LoadRL);
				this.AttachDelegate("Load", () => Loads);
			}

			// Load settings
			Settings = this.ReadCommonSettings<Settings>("GeneralSettings", () => new Settings());

			this.AttachDelegate("Gain", () => Settings.Gain);

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
