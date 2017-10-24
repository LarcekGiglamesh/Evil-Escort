using UnityEngine;
using System.Collections;

public class StringManager
{
	public class Singleton
	{
		public const string gameSystems = "GameSystems";
		public const string playerVarsManager = "PlayerVarsManager";
		public const string gameManager = "GameManager";
		public const string gameVarsManager = "GameVarsManager";
		public const string audioManager = "AudioManager";
		public const string ingameMenu_Canvas = "IngameMenu_Canvas";
	}

	public class Names
	{
		public const string rageBarFG = "RageBarFG";

		public const string levelSlider = "Slider";
		public const string levelLeftBorder = "BorderLeft_Object";
		public const string levelRightBorder = "BorderRight_Object";
		public const string cameraTargetRedRidingHood = "CameraTargetRedRidingHood";
		public const string cameraTargetPlayer = "CameraTarget";
		public const string worldMap = "WorldMap";

		public const string cameraLimitLeft = "CameraLimit_Left";
		public const string cameraLimitRight = "CameraLimit_Right";
		public const string cameraLimitTop = "CameraLimit_Top";
		public const string cameraLimitBottom = "CameraLimit_Bottom";

		public const string collectableCounter = "CollectableCount";
	    public const string worldMapTextCollectables = "TextCollectables";
	    public const string worldMapTextTries = "TextTries";
	    public const string worldMapTextBestTime = "TextBestTime";
	}

	public class Tags
	{
		public const string cameraTarget = "CameraTarget";
		public const string player = "Player";
		public const string redRidingHood = "RedRidingHood";
		public const string basicAttack = "BasicAttack";
		public const string obstacle = "Obstacle";
		public const string fallingObstacle = "FallingObstacle";
		public const string platformJumpThrough = "PlatformJumpThrough";
		public const string enemy = "Enemy";
		public const string jumpPlatform = "JumpPlatform";
		public const string pickupObj = "PickupObj";
		public const string door = "Door";
		public const string cameraLevelOverviewPoint = "CameraLevelOverviewPoint";
		public const string cameraLimitRect = "CameraLimitRect";
    }

	public class Resources
	{
		public const string gameSystems = "GameSystems";
		public const string basicAttack = "Mover/BasicAttack";
	}
}
