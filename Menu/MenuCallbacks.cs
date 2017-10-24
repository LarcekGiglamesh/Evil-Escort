using UnityEngine;
using System.Collections;
// using UnityEditor;

public class MenuCallbacks : MonoBehaviour
{
    [Tooltip("Wartezeit in Sekunden bevor der Callback der Folge Funktion (bei Maus Klick) stattfindet.")]
    public float m_WaitBeforePressedAction = 1.0f;

	private enum MenuStates { Menu, Start, History, Credits, Exit }
	private MenuStates m_MenuState = MenuStates.Menu;

    [System.Serializable]
    public struct HistoryPages
    {
        public GameObject m_Parent;
        public Texture2D m_LeftSide;
        public Texture2D m_RightSide;
    }

    [System.Serializable]
    public struct HistoryReward
    {
        public int m_NecessaeyRewardCount;
        public int m_ChapterNumber;
        public Texture2D m_ChoosenTexture;
    }

    // Bookmark Lerp
    public GameObject m_BookmarkObject = null;
    public Color m_BookmarkLerpBegin;
    public Color m_BookmarkLerpEnd;
    private float m_FadingValue = 0.0f;
    private float m_FadingKey = 1.0f;

    // ROOT of Book
    public GameObject m_BookRoot = null;
    public GameObject m_MainPage = null;

    // GameObjects to draw the Textures on
    public GameObject m_BookbackgroundLeft = null;
    public GameObject m_BookbackgroundRight = null;
    public GameObject m_AnimatedPageFront = null;
    public GameObject m_AnimatedPageBack = null;

    // Same Scenes but other States
    public HistoryPages[] m_PagesHistory = null;
    public Texture2D m_PageCreditsLeft = null;
    public Texture2D m_PageCreditsRight = null;
    private int m_HistoryPagesCount = 0;
    public Texture2D m_WorldMapLeft = null;
    public Texture2D m_WorldMapRight = null;

    // All Textures to be Loaded
    public Texture2D m_MainMenuBase = null;
    public Texture2D m_PlayStartHover = null;
    public Texture2D m_PlayStartClick = null;
    public Texture2D m_HistoryHover = null;
    public Texture2D m_HistoryClick = null;
    public Texture2D m_CreditsHover = null;
    public Texture2D m_CreditsClick = null;
    public Texture2D m_ExitHover = null;
    public Texture2D m_ExitClick = null;
	// Last Used Textures for Page Return
	private Texture2D m_LastPageLeft = null;
	private Texture2D m_LastPageRight = null;

    // 
    public HistoryReward[] m_HistoryRewards = null; // = new HistoryReward[30];

    // Animation Controller
    private Animator m_BookAnimator = null;

    // Menu is on History?
    // private bool m_SceneIsOnHistoryPages = false;
    private int m_CurrentHistoryPage = -1;

    void Start()
    {
        m_BookAnimator = m_BookRoot.GetComponent<Animator>();
        if (m_BookAnimator == null)
        {
            Debug.LogError("Book: No Animator Attached!");
        }

        m_AnimatedPageBack.SetActive(false);
        m_AnimatedPageFront.SetActive(false);
        m_HistoryPagesCount = m_PagesHistory.Length;
    }

    void Update()
    {
        if (m_MenuState == MenuStates.History || m_MenuState == MenuStates.Credits)
        {
            m_FadingValue += ( Time.deltaTime * m_FadingKey );
            if (m_FadingValue > 1.0f) m_FadingKey = -1.0f;
            if (m_FadingValue < 0.0f) m_FadingKey = 1.0f;
            m_BookmarkObject.GetComponent<Renderer>().material.color = Vector4.Lerp(m_BookmarkLerpBegin, m_BookmarkLerpEnd, m_FadingValue);
        }
    }
	
    // 
    public void ChangeBackground(int a_Value)
    {
		switch (a_Value)
		{
			case 0:
				// Spiel Spielen
				m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_PlayStartHover;
				m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_PlayStartHover;
				break;

			case 2:
				// Geschichte
				m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_HistoryHover;
				m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_HistoryHover;
                break;

			case 4:
				// Credits
				m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_CreditsHover;
				m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_CreditsHover;
				break;

			case 6:
				// Exit
				m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_ExitHover;
				m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_ExitHover;
				break;

			default:
				Debug.LogError("Unhandled Menu Click");
				break;
		}
    }

	// This function is only Called when the Scene has to be Switched
	private void loadAnotherScene()
	{
		SaveGame.Load();
		Application.LoadLevel(GameConstants.LEVEL_ID_WORLDMAP);
	}

	public void returnToMain()
    {
		if(m_MenuState == MenuStates.Menu)
		{
			Debug.LogWarning("Already on the Main Screen; No Return to Main Screen necessary.");
			return;
		}else if(m_MenuState == MenuStates.Exit)
		{
			Debug.LogError("Game is Currently being Shut Down; Action aborted.");
			return;
		}else if (m_MenuState == MenuStates.Start)
		{
			Debug.LogWarning("Another Scene will be loaded shortly; Action aborted.");
			return;
		}
        
        // Sound Menu Click
	    Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        // Display turning Pages
        m_AnimatedPageBack.SetActive(true);
        m_AnimatedPageFront.SetActive(true);
        // do Animation
        m_BookAnimator.SetTrigger("turnLeft");
		// set background Textures
		m_AnimatedPageFront.GetComponent<Renderer>().material.mainTexture = m_MainMenuBase;
		m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_MainMenuBase;
		// Set turning Pages Textures
		m_AnimatedPageBack.GetComponent<Renderer>().material.mainTexture = m_LastPageLeft;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_LastPageRight;
		// Return State
		m_MenuState = MenuStates.Menu;
		// Wait 1 Second before resetting
		Invoke("resetScene", 1.0f); // 1.0 ~= 24 Frames / 24.44 Frames per Second
    }

    public void resetScene()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

	// *******************************************************************************************
	//                  END GAME
	// *******************************************************************************************

	public void endGame()
	{
        // Sound Menu Click
        Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        //TODO: Safety Dialog
        // Remember State for Bookmark Actions

        m_MenuState = MenuStates.Exit;
		Debug.Log("GAME END TRIGGERED");
        Application.Quit();

/*#if UNITY_EDITOR
        EditorApplication.ExecuteMenuItem("Edit/Play");
#endif*/
    }


    // *******************************************************************************************
    //                  START GAME
    // *******************************************************************************************

    public void ChangeToStart()
    {
		// Remember State for Bookmark Actions
		m_MenuState = MenuStates.Start;
        // Sound Menu Click
        Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        // Disable Main Page
        m_MainPage.SetActive(false);
        // Display turning Pages
        m_AnimatedPageBack.SetActive(true);
        m_AnimatedPageFront.SetActive(true);
        // do Animation
        m_BookAnimator.SetTrigger("turnRight");
        // set background Textures
        m_AnimatedPageFront.GetComponent<Renderer>().material.mainTexture = m_PlayStartClick;
        m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_PlayStartClick;
        // Set turning Pages Textures
        m_AnimatedPageBack.GetComponent<Renderer>().material.mainTexture = m_WorldMapLeft;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_WorldMapRight;
        // Wait 1 Second before resetting
        Invoke("ChangeToStart_After", 1.0f);
    }

    private void ChangeToStart_After()
    {
        // hide turning Pages
        m_AnimatedPageBack.SetActive(false);
        m_AnimatedPageFront.SetActive(false);
        // display History Pages
        m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_WorldMapLeft;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_WorldMapRight;

        if (m_CurrentHistoryPage != -1)
        {
            m_PagesHistory[m_CurrentHistoryPage].m_Parent.SetActive(true);
        }

		Invoke("loadAnotherScene", m_WaitBeforePressedAction);
	}


    // *******************************************************************************************
    //                  HISTORY
    // *******************************************************************************************

    public void ChangeToHistory()
    {
		// Remember State for Bookmark Actions
		m_MenuState = MenuStates.History;
        // Sound Menu Click
        Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        // Load Textures in Relation to Unlocked Rewards
        ManipulateHistoryPages();

        // Disable Main Page
        m_MainPage.SetActive(false);
        // Display turning Pages
        m_AnimatedPageBack.SetActive(true);
        m_AnimatedPageFront.SetActive(true);
        // do Animation
        m_BookAnimator.SetTrigger("turnRight");
        // set background Textures
        m_AnimatedPageFront.GetComponent<Renderer>().material.mainTexture = m_HistoryClick;
        m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_HistoryClick;
        // Set turning Pages Textures
        m_AnimatedPageBack.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[0].m_LeftSide;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[0].m_RightSide;
        // Wait 1 Second before resetting
        Invoke("ChangeToHistory_After", 1.0f);
    }

    private void ChangeToHistory_After()
    {
        // hide turning Pages
        m_AnimatedPageBack.SetActive(false);
        m_AnimatedPageFront.SetActive(false);
        // display History Pages
        m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[0].m_LeftSide;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[0].m_RightSide;
		// Remember for Return to Main
		m_LastPageLeft = m_PagesHistory[0].m_LeftSide;
		m_LastPageRight = m_PagesHistory[0].m_RightSide;

		// m_SceneIsOnHistoryPages = true;
        m_CurrentHistoryPage = 0;
        m_PagesHistory[m_CurrentHistoryPage].m_Parent.SetActive(true);
    }

    public void ChangeToNextHistoryPage()
    {
        if ((m_CurrentHistoryPage + 1) >= m_HistoryPagesCount)
        {
            Debug.LogWarning("No Next History Page Found.");
            return;
        }

        // Sound Menu Click
        Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        m_PagesHistory[m_CurrentHistoryPage].m_Parent.SetActive(false);

		// Display turning Pages
		m_AnimatedPageBack.SetActive(true);
		m_AnimatedPageFront.SetActive(true);

		// set background Textures
		m_AnimatedPageFront.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[m_CurrentHistoryPage].m_RightSide;
		m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[m_CurrentHistoryPage].m_LeftSide;
		m_CurrentHistoryPage++;
		// Set turning Pages Textures
		m_AnimatedPageBack.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[m_CurrentHistoryPage].m_LeftSide;
		m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[m_CurrentHistoryPage].m_RightSide;
		// do Animation
		m_BookAnimator.SetTrigger("turnRight");

		Invoke("ChangeToNextHistoryPage_After", 1.0f);
    }

	public void ChangeToNextHistoryPage_After()
	{
		// hide turning Pages
		m_AnimatedPageBack.SetActive(false);
		m_AnimatedPageFront.SetActive(false);

		// Remember for Return to Main
		m_LastPageLeft = m_PagesHistory[m_CurrentHistoryPage].m_LeftSide;
		m_LastPageRight = m_PagesHistory[m_CurrentHistoryPage].m_RightSide;

		// Set Textures for Current Page
		m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_LastPageLeft;
		m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_LastPageRight;

		m_PagesHistory[m_CurrentHistoryPage].m_Parent.SetActive(true);
		// Debug.Log("[NEXT] Changed to History Page '" + m_CurrentHistoryPage + "'");
	}

	public void ChangeToLastHistoryPage()
    {
        if ((m_CurrentHistoryPage - 1) < 0)
        {
            Debug.LogWarning("No Last History Page Found.");
            return;
        }

        // Sound Menu Click
        Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        m_PagesHistory[m_CurrentHistoryPage].m_Parent.SetActive(false);

		// Display turning Pages
		m_AnimatedPageBack.SetActive(true);
		m_AnimatedPageFront.SetActive(true);

        m_CurrentHistoryPage--;
        // Set turning Pages Textures
        m_AnimatedPageBack.GetComponent<Renderer>().material.mainTexture = m_LastPageLeft;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_LastPageRight;
        // set background Textures
        m_AnimatedPageFront.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[m_CurrentHistoryPage].m_RightSide;
		m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_PagesHistory[m_CurrentHistoryPage].m_LeftSide;
		
		// do Animation
		m_BookAnimator.SetTrigger("turnLeft");

		Invoke("ChangeToLastHistoryPage_After", 1.0f);
	}

	public void ChangeToLastHistoryPage_After()
	{
		// hide turning Pages
		m_AnimatedPageBack.SetActive(false);
		m_AnimatedPageFront.SetActive(false);

		// Remember for Return to Main
		m_LastPageLeft = m_PagesHistory[m_CurrentHistoryPage].m_LeftSide;
		m_LastPageRight = m_PagesHistory[m_CurrentHistoryPage].m_RightSide;

		// Set Textures for Current Page
		m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_LastPageLeft;
		m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_LastPageRight;

		m_PagesHistory[m_CurrentHistoryPage].m_Parent.SetActive(true);
		// Debug.Log("[NEXT] Changed to History Page '" + m_CurrentHistoryPage + "'");
	}

	// *******************************************************************************************
	//                  CREDITS
	// *******************************************************************************************

	public void ChangeToCredits()
    {
		// Remember State for Bookmark Actions
		m_MenuState = MenuStates.Credits;
        // Sound Menu Click
        Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

        // Disable Main Page
        m_MainPage.SetActive(false);
        // Display turning Pages
        m_AnimatedPageBack.SetActive(true);
        m_AnimatedPageFront.SetActive(true);
        // do Animation
        m_BookAnimator.SetTrigger("turnRight");
        // set background Textures
        m_AnimatedPageFront.GetComponent<Renderer>().material.mainTexture = m_CreditsClick;
        m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_CreditsClick;
        // Set turning Pages Textures
        m_AnimatedPageBack.GetComponent<Renderer>().material.mainTexture = m_PageCreditsLeft;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_PageCreditsRight;
        // Wait 1 Second before resetting
        Invoke("ChangeToCredits_After", 1.0f);
    }

    private void ChangeToCredits_After()
    {
        // hide turning Pages
        m_AnimatedPageBack.SetActive(false);
        m_AnimatedPageFront.SetActive(false);
        // display History Pages
        m_BookbackgroundLeft.GetComponent<Renderer>().material.mainTexture = m_PageCreditsLeft;
        m_BookbackgroundRight.GetComponent<Renderer>().material.mainTexture = m_PageCreditsRight;
		// Remember for Return to Main
		m_LastPageLeft = m_PageCreditsLeft;
		m_LastPageRight = m_PageCreditsRight;
	}

    // *******************************************************************************************
    //
    // *******************************************************************************************


    public void ManipulateHistoryPages()
    {
        if (m_PagesHistory.Length < 5)
        {
            Debug.LogError("Too few Pages in Existence. Function Aborted.");
            return;
        }

        SaveGame.Load();
        bool[] rewardsUnlocked = SaveGame.GetData().rewardsUnlocked;
        int NumberOfRewards = rewardsUnlocked.Length;
        int CountNumberOfCollectedRewards = 0;
        for (int i = 0; i < NumberOfRewards; i++)
        {
            if (rewardsUnlocked[i] == true)
            {
                CountNumberOfCollectedRewards++;
            }
        }

        if (CountNumberOfCollectedRewards == 0)
        {
            Debug.LogWarning("No rewards Collected so far. Aborting Actions.");
            return;
        }

        // Debug.Log("Number of Rewards Collected: " + CountNumberOfCollectedRewards);
        int curChapter = -1;
        for (int i = 0; i < 18; i++)
        {
            if (CountNumberOfCollectedRewards > i)
            {
                curChapter = m_HistoryRewards[i].m_ChapterNumber;
                m_PagesHistory[curChapter].m_LeftSide = m_HistoryRewards[i].m_ChoosenTexture;
                m_PagesHistory[curChapter].m_RightSide = m_HistoryRewards[i].m_ChoosenTexture;
            }
        }
        
    }

}
