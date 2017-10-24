using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class IngameMenu : MonoBehaviour
{

	public enum MenuEvents
	{
		ContinueLevel,
		ContinueNextLevel,
		RetryLevel,
		SwitchToMainMenu,
		SwitchToLevelMenu,
		ExitGame,
		ExitGameSureYes,
		ExitGameSureNo
	}

	[System.Serializable]
	public struct ButtonEvent
	{
		public Button m_Button;
		public MenuEvents m_Action;
	}

	public ButtonEvent[] m_ButtonEvents;
	public GameObject m_ExitGameSafetyDialoge;
	
	private void ContinueLevel()
	{
		// Debug.Log("Continue Level");
		this.transform.parent.gameObject.SetActive(false);
		Time.timeScale = 1.0f;
		return;
	}


	private void ContinueNextLevel()
	{
		// Go back to the WorldMap Scene
		Application.LoadLevel(GameConstants.LEVEL_ID_WORLDMAP);
		Singleton.AudioManager.StopBGM();
		Singleton.AudioManager.PlayBGM(BGMName.mainMenu, 0.0f);
		Time.timeScale = 1.0f;
		return;
	}


	private void RetryLevel()
	{
		Singleton.AudioManager.StopBGM();
		Singleton.AudioManager.PlayBGM(BGMName.ingame, 0.0f);

		Application.LoadLevel(Application.loadedLevel);
		Time.timeScale = 1.0f;
	}

	private void SwitchToMainMenu()
	{
		// Debug.Log("Switch to Main");
		Application.LoadLevel(0);
		Time.timeScale = 1.0f;

		Singleton.AudioManager.StopBGM();
		Singleton.AudioManager.PlayBGM(BGMName.mainMenu, 0.0f);
	}

	private void SwitchToLevelMenu()
	{
		// Debug.Log("Switch to Levels");
		Application.LoadLevel(GameConstants.LEVEL_ID_WORLDMAP);
		Time.timeScale = 1.0f;

		Singleton.AudioManager.StopBGM();
		Singleton.AudioManager.PlayBGM(BGMName.mainMenu, 0.0f);
	}



	private void ExitGame()
	{
		// Debug.Log("Exit Game");
		m_ExitGameSafetyDialoge.SetActive(true);
		Time.timeScale = 1.0f;
	}

	private void ExitGameSureYes()
	{
		// Debug.Log("Exit Game Sure Yes");
		Application.Quit();
		Time.timeScale = 1.0f;
	}

	private void ExitGameSureNo()
	{
		// Debug.Log("Exit Game Sure No");
		m_ExitGameSafetyDialoge.SetActive(false);
		Time.timeScale = 1.0f;
	}


	// Use this for initialization
	void Start () {

		int numberOfButtons = m_ButtonEvents.Length;

		for(int i = 0; i < numberOfButtons; i++)
		{
			if (m_ButtonEvents[i].m_Button == null)
				continue;

			m_ButtonEvents[i].m_Button.onClick.RemoveAllListeners();
			switch (m_ButtonEvents[i].m_Action)
			{
				case MenuEvents.ContinueLevel:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { ContinueLevel(); });
					break;

				case MenuEvents.ContinueNextLevel:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { ContinueNextLevel(); });
					break;

				case MenuEvents.RetryLevel:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { RetryLevel(); });
					break;

				case MenuEvents.SwitchToLevelMenu:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { SwitchToLevelMenu(); });
					break;

				case MenuEvents.SwitchToMainMenu:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { SwitchToMainMenu(); });
					break;

				case MenuEvents.ExitGame:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { ExitGame(); });
					break;

				case MenuEvents.ExitGameSureYes:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { ExitGameSureYes(); });
					break;

				case MenuEvents.ExitGameSureNo:
					m_ButtonEvents[i].m_Button.onClick.AddListener(delegate () { ExitGameSureNo(); });
					break;

				default:
					Debug.LogError("Unhandled Button Event");
					break;
			}
		}
	}
	
	
}
