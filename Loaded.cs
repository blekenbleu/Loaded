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
    public class Loaded : IPlugin, IDataPlugin, IWPFSettingsV2
    {
		string GameDBText, LoadStr;
		double LoadFL, LoadFR, LoadRL, LoadRR, Heave;
		string[] corner;
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
        /// Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName attribute.
        /// </summary>
        public string LeftMenuTitle => "Loaded";

        /// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///
        /// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        ///
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // Define the value of our property (declared in init)
            if (data.GameRunning)
            {
                if (data.OldData != null && data.NewData != null)
                {
					Heave = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameData.AccelerationHeave"));
					if (null != LoadStr)
		            {
						LoadFR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[0]));
						LoadFL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[1]));
						LoadRR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[2]));
						LoadRL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[3]));
		            }
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
        Control View;
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
			GameDBText = pluginManager.GameName;
            this.AttachDelegate("Game", () => GameDBText);
			switch (GameDBText)
			{
                case "AssettoCorsa":
					LoadStr = "DataCorePlugin.GameRawData.Physics.WheelLoad0";
					corner =  new string[] { "1", "2", "3", "4" };
                    break;
			}

			this.AttachDelegate("Heave", () => Heave);
			if (null != LoadStr)
			{
				this.AttachDelegate("FR", () => LoadFR);
				this.AttachDelegate("FL", () => LoadFL);
				this.AttachDelegate("RR", () => LoadRR);
				this.AttachDelegate("RL", () => LoadRL);
			}
            // Load settings
            Settings = this.ReadCommonSettings<Settings>("GeneralSettings", () => new Settings());

            // Declare a property available in the property list, this gets evaluated "on demand" (when shown or used in formulas)
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
