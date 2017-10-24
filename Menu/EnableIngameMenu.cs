using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnableIngameMenu : MonoBehaviour {

	public GameObject m_MenuPause;
	public GameObject m_MenuLevelEndGood;
	public GameObject m_MenuLevelEndBad;
	public GameObject m_UserInterface;

	void Start()
	{
		StatsTracker.OnLevelLoaded();
		// Disable UI in Tutorial Level until RRH_Trigger activates it
		if (Application.loadedLevel == 2)
		{
			Debug.Log("TUTORIAL LEVEL -> UserInterface disabled!");
			disableAll();
		}
	}

	public void disableAll()
	{
		m_MenuLevelEndBad.SetActive(false);
		m_MenuLevelEndGood.SetActive(false);
		m_MenuPause.SetActive(false);
		m_UserInterface.SetActive(false);
	}
	
	public void displayPauseMenu()
	{
		m_MenuPause.SetActive(true);
	}


	public void displayLevelFailed()
	{
		m_MenuLevelEndBad.SetActive(true);
	}

	public void displayLevelFailed(string strObjective01, string strObjective02, string strObjective03)
	{
		GameObject.Find("_DeathScreenObj01Value").GetComponent<Text>().text = strObjective01;
		GameObject.Find("_DeathScreenObj02Value").GetComponent<Text>().text = strObjective02;
		GameObject.Find("_DeathScreenObj03Value").GetComponent<Text>().text = strObjective03;
		displayLevelFailed();
	}

	public void displayLevelFinished()
	{
		m_MenuLevelEndGood.SetActive(true);
	}

	public void displayLevelFinished(string strObjective01, string strObjective02, string strObjective03)
	{
		GameObject.Find("_WinScreenObj01Value").GetComponent<Text>().text = strObjective01;
		GameObject.Find("_WinScreenObj02Value").GetComponent<Text>().text = strObjective02;
		GameObject.Find("_WinScreenObj03Value").GetComponent<Text>().text = strObjective03;
		displayLevelFinished();
	}

	public void displayUserInterface()
	{
		m_UserInterface.SetActive(true);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Escape))
		{
			Time.timeScale = 0.0f;
			displayPauseMenu();
		}

		if (Input.GetKey(KeyCode.Keypad2))
		{
			displayLevelFailed();
		}

		if (Input.GetKey(KeyCode.Keypad3))
		{
			displayLevelFinished();
		}

		if (Input.GetKey(KeyCode.Keypad4))
		{
			displayUserInterface();
		}

		if (Input.GetKey(KeyCode.Keypad0))
		{
			disableAll();
		}
	}
}
