using UnityEngine;
using System.Collections;

public class DebugFunctions : MonoBehaviour
{

	void Start()
	{
	
	}
	
	void Update()
	{
#if DEBUG
		// Reload Level
		if (Input.GetKeyDown(KeyCode.R) && Application.loadedLevel > GameConstants.LEVEL_ID_WORLDMAP)
		{
			Application.LoadLevel(Application.loadedLevel);
			return;
		}
#endif
		//// Back to WorldMap
		//if(Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel > GameConstants.LEVEL_ID_WORLDMAP)
		//{
		//	Application.LoadLevel(GameConstants.LEVEL_ID_WORLDMAP);
		//	return;
		//}
		//// Back to MainMenu
		//if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel == GameConstants.LEVEL_ID_WORLDMAP)
		//{
		//	Application.LoadLevel(GameConstants.LEVEL_ID_MAINMENU);
		//	return;
		//}
		//// Change to next World
		//if (Application.loadedLevel == 0 && Input.GetKeyDown(KeyCode.N))
		//{
		//	WorldMap worldMap = GameObject.Find(StringManager.Names.worldMap).GetComponent<WorldMap>();
		//	worldMap.ChangeToWorld(worldMap.GetCurrentWorldIndex() + GameConstants.LEVEL_ID_OFFSET);
		//	return;
		//}
		//// Save Game
		//if(Input.GetKeyDown(KeyCode.F1))
		//{
		//	SaveGame.Save();
		//	return;
		//}
		//// Load SaveGame
		//if(Input.GetKeyDown(KeyCode.F2))
		//{
		//	SaveGame.Load();
		//	return;
		//}
	}
}
