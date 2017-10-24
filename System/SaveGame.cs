using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

public class SaveData
{
	public int worldMapWorldIndex;
	public int worldMapPointIndex;
	public int levelIndexUnlocked;

	public bool[] rewardsUnlocked = new bool[GameConstants.REWARDS_NUM];
	public float[] bestTimes = new float[GameConstants.MAX_LEVEL_INDEX]; // evtl. -1
	public int[] numTries = new int[GameConstants.MAX_LEVEL_INDEX];
	public int[] numRageBarActivated = new int[GameConstants.MAX_LEVEL_INDEX];
}

// Track Stats of current Level
public static class StatsTracker
{
	public static int m_previousSceneIndex = -1;

	public static float m_timeLevelStarted;
	public static int m_numTries;
	public static int m_numRageBarActivated;
	public static int m_numRewardsCollected;

	public static void OnLevelLoaded()
	{
		int newSceneIndex = Application.loadedLevel;
		// If a new Scene was loaded
		if (newSceneIndex != m_previousSceneIndex || m_previousSceneIndex == -1)
		{
			m_numTries = 1;
        }
		else // If Current Scene was reloaded
		{
			m_numTries++;
		}
		m_timeLevelStarted = Time.time;
		m_numRageBarActivated = 0;
		m_numRewardsCollected = 0;

		m_previousSceneIndex = newSceneIndex;
    }

	public static void OnLevelSuccess()
	{
		// Debug.Log("Continue Next Level");
		int finishedLevel = SaveGame.GetCurrentLevelIndex();

		// Level Unlock
		if (SaveGame.GetData().levelIndexUnlocked == finishedLevel)
		{
			SaveGame.GetData().levelIndexUnlocked++;
			if (SaveGame.GetData().levelIndexUnlocked > GameConstants.MAX_LEVEL_INDEX)
			{
				SaveGame.GetData().levelIndexUnlocked = GameConstants.MAX_LEVEL_INDEX;
			}
		}

		// BestTime for this Level
		float bestTime = SaveGame.GetData().bestTimes[finishedLevel];
		float currentTime = Time.time - m_timeLevelStarted;
		if (currentTime < bestTime || bestTime == 0.0f)
		{
			SaveGame.GetData().bestTimes[finishedLevel] = currentTime;
			// Trys (for BestTime)
			SaveGame.GetData().numTries[finishedLevel] = m_numTries;
		}
		Debug.Log("current Time: " + currentTime);
		Debug.Log("prev best Time: " + bestTime);
		Debug.Log("best Time: " + SaveGame.GetData().bestTimes[finishedLevel]);


		// Show Ingame UI
		Singleton.IngameMenu.m_MenuLevelEndGood.SetActive(true);
		Singleton.IngameMenu.m_UserInterface.SetActive(false);
		string collectedRewards = m_numRewardsCollected.ToString();
		string timeSpentOnLevel = currentTime.ToString();
		string rageBarActivated = m_numRageBarActivated.ToString();
		Singleton.IngameMenu.displayLevelFinished(collectedRewards, timeSpentOnLevel, rageBarActivated);
		
		// Auto Save
		SaveGame.Save();
	}

	public static void OnLevelFailed()
	{
		//TODO: Todesart, Zeit, Ragebar in TXT

		Singleton.IngameMenu.m_MenuLevelEndBad.SetActive(true);
		Singleton.IngameMenu.m_UserInterface.SetActive(false);

		string reasonForDeath = "Fell out of the World";
		string timeSpentOnLevel = (Time.time - m_timeLevelStarted).ToString();
		string rageBarActivated = m_numRageBarActivated.ToString();

		Singleton.IngameMenu.displayLevelFailed(reasonForDeath, timeSpentOnLevel, rageBarActivated);

		Singleton.AudioManager.StopBGM();
		Singleton.AudioManager.PlayBGM(BGMName.deathScreen, 0.0f);
	}
}

public static class SaveGame
{
	private const string m_path = "savegame.xml";
	private static SaveData m_saveData = new SaveData();

	public static void Save()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
		FileStream file = new FileStream(m_path, FileMode.Create);
		serializer.Serialize(file, m_saveData);
		file.Close();
    }

	public static void Load()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
		FileStream file = new FileStream(m_path, FileMode.OpenOrCreate);
		try
		{
			m_saveData = (SaveData)serializer.Deserialize(file);
		}
		catch
		{
			Debug.Log("Could not deserialize the SaveGame.xml!");
		}
		file.Close();
	}

	public static SaveData GetData()
	{
		return m_saveData;
	}

	public static int GetNumRewardsCollected(int levelIndex)
	{
		int numRewards = 0;
		// Skip Rewards from previous Levels
		int start = levelIndex * GameConstants.REWARDS_PER_LEVEL;
		int end = start + GameConstants.REWARDS_PER_LEVEL;
		if (end >= m_saveData.rewardsUnlocked.Length)
		{
			Debug.LogError("GetNumRewardsCollected(int sceneIndex) => End Index is higher then Max Reward Index! end = " + end);
			return -1;
		}
		// Loop through Rewards of current Level
		for (int i = start; i < end; i++)
		{
			if (m_saveData.rewardsUnlocked[i])
				numRewards++;
		}
		return numRewards;
	}

	public static int GetCurrentLevelIndex()
	{
		return (Application.loadedLevel - GameConstants.LEVEL_ID_OFFSET);
	}
}
