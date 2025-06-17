 "RearLeftFormula":
{
	"JSExt": 0,
	"Interpreter": 1,
	"Expression": "
```
	// Dont waste processing time if in pits or in pit lane or is less than 5kph$prop('SpeedKmh')
	// if ($prop('IsInPit')==1 || $prop('IsInPitLane')==1 || $prop('SpeedKmh')<=5)	return 0;

	var degreeRad = Math.PI/180; // 360 degrees to 2 * $\pi$ radians

	// Angular velocities in radians per second
	var yaw_rate = $prop('GameRawData.mAngularVelocity02');

	// Normalized steering input
	var steeringInput = $prop('GameRawData.mSteering'); // [ RANGE = -1.0f->1.0f ]
	var steeringAngleDeg = steeringInput * 24; // Typical steering angle 24 degrees, match to what is set in game

	// Convert normalized steering input to actual steering angle in radians
	var steeringAngleRad = steeringAngleDeg * degreeRad;

	// Local velocities in metres per second
	var Vlateral = $prop('GameRawData.mLocalVelocity01');
	var Vlong = $prop('GameRawData.mLocalVelocity03');

	// Assumed distances to front and rear axles in meters not available in game
	var L_f = 1.2; // Distance to front axle [ UNITS = Metres ]
	var L_r = 1.6; // Distance to rear axle [ UNITS = Metres ]

	// Calculate front slip angle in radians from simplified equation
	var alpha_f = steeringAngleRad - Math.atan((Vlateral + L_f * yaw_rate) / Vlong);

	// Calculate rear slip angle in radians from simplified equation
	var alpha_r = -Math.atan((Vlateral - L_r * yaw_rate) / Vlong);

	// Convert slip angles from radians to degrees
	var alpha_f_deg = alpha_f * (180 / Math.PI);
	var alpha_r_deg = alpha_r * (180 / Math.PI);

	// Output rear slip in degrees
	var pit = $prop('IsInPit');
	var game =  NewRawData().mGameState;
	if(pit !=1 && game!=4 )
	{	// prevent from making noise if not driving
		return Math.abs(alpha_r_deg *2);
	}
	return 0;
```
	"
}
