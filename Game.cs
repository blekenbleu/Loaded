namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		void Game (string name)
		{
			switch (name)
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
					dorner =  new string[] { "01.suspensionDeflection", "02.suspensionDeflection",
											 "04.suspensionDeflection", "04.suspensionDeflection" };
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
					dorner =  new string[] { "1.mVerticalTireDeflection", "2.mVerticalTireDeflection",
											 "3.mVerticalTireDeflection", "4.mVerticalTireDeflection" };
					break;
				case "RBR":
					LoadStr = "DataCorePlugin.GameRawData.NGPTelemetry.car.suspension";
					DeflStr = "DataCorePlugin.GameRawData.NGPTelemetry.car.suspension";
					corner =  new string[] { "LF.strutForce", "RF.strutForce", "LB.strutForce", "RB.strutForce" };
					dorner =  new string[] { "LF.springDeflection", "RF.springDeflection",
											 "LB.springDeflection", "RB.springDeflection" };
					break;
			}
		}
	}
}
