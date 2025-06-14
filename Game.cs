namespace blekenbleu.loaded
{
	public partial class Loaded
	{
		string Psteer = "", Psway = "";

		void Game (string name, string raw)
		{
			switch (name)
			{
				case "AssettoCorsa":
					LoadStr = raw+"Physics.WheelLoad0";
					DeflStr = raw+"Physics.SuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					Psteer = raw+"Physics.SteerAngle";
					break;
				case "AssettoCorsaCompetizione":
					DeflStr = raw+"Physics.SuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					Psteer = raw+"Physics.SteerAngle";
					Psway = raw+"Physics.LocalVelocity01";
					break;
				case "Automobilista":
					LoadStr = raw+"Data.wheel";
					DeflStr = raw+"Data.wheel";
					corner =  new string[] { "01.tireLoad", "02.tireLoad", "04.tireLoad", "04.tireLoad" };
					dorner =  new string[] { "01.suspensionDeflection", "02.suspensionDeflection",
											 "04.suspensionDeflection", "04.suspensionDeflection" };
					Psteer = raw+"Data.unfilteredSteering";
					Psway = raw+"Data.localVel.x";
					break;
				case "Automobilista2":
					DeflStr = raw+"mSuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					Psteer = raw+"mUnfilteredSteering";
					Psway = raw+"mLocalVelocity01";
					break;
				case "BeamNgDrive":
					DeflStr = raw+"suspension_position_";
					corner =  new string[] { "fl", "fr", "rl", "rr" };
					Psteer = raw+"input_steeringPercent";
					break;
				case "CodemastersDirt4":
					DeflStr = raw+"SuspensionPosition";
					corner =  new string[] { "FrontLeft", "FrontRight", "RearLeft", "RearRight" };
					Psteer = raw+"Steer";
					// calculate from Speed, WorldSpeedX, WorldSpeedY
					break;
				case "EAWRC23":
					DeflStr = raw+"SessionUpdate.vehicle_hub_position_";
					corner =  new string[] { "fl", "fr", "bl", "br" };
					Psteer = raw+"SessionUpdate.vehicle_steering";
					Psway = raw+"SessionUpdateLocalVelocity.X";
					break;
				case "IRacing":
					Psteer = raw+"Telemetry.SteeringWheelAngle";
					break;
				case "FH5":
					DeflStr = raw+"NormalizedSuspensionTravel";
					corner =  new string[] { "FrontLeft", "FrontRight", "RearLeft", "RearRight" };
					break;
				case "PCars2":
					DeflStr = raw+"mSuspensionTravel0";
					corner =  new string[] { "1", "2", "3", "4" };
					Psteer = raw+"mSteering";
					Psway = raw+"mLocalVelocity01";
					break;
				case "RRRE":
					DeflStr = raw+"Player.SuspensionDeflection.";
					corner =  new string[] { "FrontLeft", "FrontRight", "RearLeft", "RearRight" };
					Psteer = raw+"SteerInputRaw";
					Psway = raw+"Player.LocalVelocity.X";
					break;
				case "RFactor2":
					LoadStr = raw+"CurrentPlayerTelemetry.mWheels0";
					DeflStr = raw+"CurrentPlayerTelemetry.mWheels0";
					corner =  new string[] { "1.mSuspForce", "2.mSuspForce", "3.mSuspForce", "4.mSuspForce" };
					dorner =  new string[] { "1.mVerticalTireDeflection", "2.mVerticalTireDeflection",
											 "3.mVerticalTireDeflection", "4.mVerticalTireDeflection" };
					Psteer = raw+"CurrentPlayerTelemetry.mFilteredSteering";
					Psway = raw+"CurrentPlayerTelemetry.mLocalVel.x";
					break;
				case "RBR":
					LoadStr = raw+"NGPTelemetry.car.suspension";
					DeflStr = raw+"NGPTelemetry.car.suspension";
					corner =  new string[] { "LF.strutForce", "RF.strutForce", "LB.strutForce", "RB.strutForce" };
					dorner =  new string[] { "LF.springDeflection", "RF.springDeflection",
											 "LB.springDeflection", "RB.springDeflection" };
					Psteer = raw+"Steering";
					Psway = raw+"NGPTelemetry.car.velocities.sway";
					break;
				default:
					Psteer = Psway = "";
					break;
			}
		}
	}
}
