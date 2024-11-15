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
		string GameDBText, LoadStr, DeflStr;
		double LoadFL, LoadFR, LoadRL, LoadRR, DeflFL, DeflFR, DeflRL, DeflRR, Heave;
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
						if (null != corner)
						{
							LoadFR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[0]));
							LoadFL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[1]));
							LoadRR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[2]));
							LoadRL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+corner[3]));
						}
						else {
							LoadFR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[0]));
							LoadFL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[1]));
							LoadRR = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[2]));
							LoadRL = Convert.ToDouble(pluginManager.GetPropertyValue(LoadStr+dorner[3]));
						}
					}
					if (null != DeflStr)
					{
						if (null != dorner)
						{
							DeflFR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[0]));
							DeflFL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[1]));
							DeflRR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[2]));
							DeflRL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+dorner[3]));
						}
						else {
							DeflFR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[0]));
							DeflFL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[1]));
							DeflRR = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[2]));
							DeflRL = Convert.ToDouble(pluginManager.GetPropertyValue(DeflStr+corner[3]));
						}
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
					DeflStr = "DataCorePlugin.GameRawData.Physics.SuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					break;
				case "AssettoCorsaCompetizione":
					DeflStr = "DataCorePlugin.GameRawData.Physics.SuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					break;
				case "Automobilista":
					LoadStr = "DataCorePlugin.GameRawData.Data.wheel";
					DeflStr = "DataCorePlugin.GameRawData.Data.wheel";
					corner =  new string[] { "01.tireLoad", "02.tireLoad", "04.tireLoad", "04.tireLoad" };
					dorner =  new string[] { "01.suspensionDeflection", "02.suspensionDeflection", "04.suspensionDeflection", "04.suspensionDeflection" };
					break;
				case "Automobilista2":
					DeflStr = "DataCorePlugin.GameRawData.mSuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					break;
				case "BeamNgDrive":
					DeflStr = "DataCorePlugin.GameRawData.suspension_position_";
					corner =  new string[] { "fl", "fr", "rl", "rr" };
					break;
				case "CodemastersDirt4":
					DeflStr = "DataCorePlugin.GameRawData.SuspensionPosition";
					corner =  new string[] { "FrontLeft", "FrontRight", "RearLeft", "RearRight" };
					break;
				case "EAWRC23":
					DeflStr = "DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_";
					corner =  new string[] { "fl", "fr", "bl", "br" };
					break;
				case "FH5":
					DeflStr = "DataCorePlugin.GameRawData.NormalizedSuspensionTravel";
					corner =  new string[] { "FrontLeft", "FrontRight", "RearLeft", "RearRight" };
					break;
				case "PCars2":
					DeflStr = "DataCorePlugin.GameRawData.mSuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					break;
				case "RRRE":
					DeflStr = "DataCorePlugin.GameRawData.Player.SuspensionDeflection.";
					corner =  new string[] { "FrontLeft", "FrontRight", "RearLeft", "RearRight" };
					break;
				case "RFactor2":
					LoadStr = "DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels0";
					DeflStr = "DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels0";
					corner =  new string[] { "1.mSuspForce", "2.mSuspForce", "3.mSuspForce", "4.mSuspForce" };
					dorner =  new string[] { "1.mVerticalTireDeflection", "2.mVerticalTireDeflection", "3.mVerticalTireDeflection", "4.mVerticalTireDeflection" };
					break;
				case "RBR":
					LoadStr = "DataCorePlugin.GameRawData.NGPTelemetry.car.suspension";
					DeflStr = "DataCorePlugin.GameRawData.NGPTelemetry.car.suspension";
					corner =  new string[] { "LF.strutForce", "RF.strutForce", "LB.strutForce", "RB.strutForce" };
					dorner =  new string[] { "LF.springDeflection", "RF.springDeflection", "LB.springDeflection", "RB.springDeflection" };
					break;
				
			}

			this.AttachDelegate("Heave", () => Heave);
			if (null != DeflStr)
			{
				this.AttachDelegate("DeflFR", () => DeflFR);
				this.AttachDelegate("DeflFL", () => DeflFL);
				this.AttachDelegate("DeflRR", () => DeflRR);
				this.AttachDelegate("DeflRL", () => DeflRL);
			}
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
