using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public struct WorldInstance
{
	public Transform[] worldPoints;
}

[System.Serializable]
public struct WorldPage
{
	public Texture2D[] texturesLeftPage;
	public Texture2D[] texturesRightPage;
}

[System.Serializable]
public struct Book
{
	public Animator animator;

	public GameObject leftPage;
	public GameObject rightPage;
	public GameObject animFrontPage;
	public GameObject animBackPage;
}

public struct BookAnimation
{
	public const float duration = 1.0f;
	public float timer;
	public bool turnLeft;
	public Texture2D nextLeftTex;
	public Texture2D nextRightTex;
}

public class WorldMap : MonoBehaviour
{
	public Transform m_playerMark;
	public Book m_book;
	public WorldPage[] m_worldPages;

	public WorldInstance[] m_worlds;
	public float m_timeBetweenPoints = 1.0f;
	public float m_vectorDiffTolerance = 0.4f;

	private int m_worldIndex = 0;
	private int m_prevPoint = 0;
	private int m_pointIndex = 0;
	private int m_maxPoint;
	private float m_inputInterval = 0.2f;
	private float m_inputTimer = 0.0f;
	private float m_moveTimer = 0.0f;

    private Text m_Collectables;
    private Text m_TryCount;
    private Text m_BestTime;

	private BookAnimation m_bookAnim;
	private GameObject m_canvas;

	void Start()
	{
		// TEST
		SaveGame.Load();

		// Set Init Indices (Saved in GameVars)
		m_worldIndex = SaveGame.GetData().worldMapWorldIndex;
		m_pointIndex = SaveGame.GetData().worldMapPointIndex;

		m_canvas = GameObject.Find("Canvas");

		// Load Text UI Elements
		m_Collectables = GameObject.Find(StringManager.Names.worldMapTextCollectables).GetComponent<Text>();
        m_BestTime = GameObject.Find(StringManager.Names.worldMapTextBestTime).GetComponent<Text>();
        m_TryCount = GameObject.Find(StringManager.Names.worldMapTextTries).GetComponent<Text>();

        InitWorld();
	}

	void InitWorld()
	{
		if (m_worlds[m_worldIndex].worldPoints.Length == 0)
		{
			Debug.LogError("WorldPoints Array does not contain any Transforms");
			return;
		}
		m_maxPoint = m_worlds[m_worldIndex].worldPoints.Length - 1;
		m_prevPoint = m_pointIndex;
		m_playerMark.position = m_worlds[m_worldIndex].worldPoints[m_pointIndex].position;


		SetAnimPageVisible(false);

		// Change Texture of Book
		ChangePageTexture(m_book.leftPage, m_worldPages[m_worldIndex].texturesLeftPage[GetTextureIndexLeftPage()]);
		ChangePageTexture(m_book.rightPage, m_worldPages[m_worldIndex].texturesRightPage[GetTextureIndexRightPage()]);
		//ChangePageTexture(m_book.animFrontPage, m_worldPages[m_worldIndex].texturesRightPage[0]);
		//ChangePageTexture(m_book.animBackPage, m_worldPages[m_worldIndex].texturesRightPage[0]);

	}

	public void ChangeToWorld(int worldIndex, int pointIndex = 0)
	{
		if(worldIndex > m_worlds.Length - 1)
		{
			Debug.LogError("WorldIndex higher than max world count!");
			return;
		}
		if (m_worldIndex == worldIndex)
		{
			Debug.LogError("ChangeToWorld() => m_worldIndex == worldIndex");
			return;
		}

		bool turnLeft = (m_worldIndex > worldIndex);

		m_worldIndex = worldIndex;
		m_pointIndex = pointIndex;
		m_inputTimer = 0.0f;
		m_moveTimer = 0.0f;

		TurnPage(turnLeft,
			m_worldPages[m_worldIndex].texturesLeftPage[GetTextureIndexLeftPage()],
			m_worldPages[m_worldIndex].texturesRightPage[GetTextureIndexRightPage()]);

		m_playerMark.gameObject.SetActive(false);
	}

	public int GetCurrentWorldIndex()
	{
		return m_worldIndex;
	}

	void Update()
	{
		if (m_bookAnim.timer <= 0.0f)
		{
			HandlePlayerWorldMovement();
			HandleInputSelectLevel();
			ShowLevelInfo();
		}

		if (m_bookAnim.timer > 0.0f)
		{
			m_bookAnim.timer -= Time.deltaTime;
			if (m_bookAnim.timer <= 0.0f)
			{
				OnPageTurnEnd();
			}
		}
	}

	void HandlePlayerWorldMovement()
	{
		if (m_inputTimer > 0.0f)
		{
			m_inputTimer -= Time.deltaTime;
		}

		if (m_inputTimer <= 0.0f && m_moveTimer == m_timeBetweenPoints)
		{
			// Get Input
			Vector2 input = GetInputVector();

			// Calc Vector Difference
			float diffLower, diffHigher;
			CalcVectorDiff(input, out diffLower, out diffHigher);

			// Update Point Index
			if (diffLower <= m_vectorDiffTolerance)
			{
				m_pointIndex--;
				if (m_pointIndex < 0)
					m_pointIndex = 0;

				ResetTimers();
			}
			else if (diffHigher <= m_vectorDiffTolerance)
			{
				m_pointIndex++;

				// Move back if level is not unlocked
				if (GetCurrentLevelIndex() > SaveGame.GetData().levelIndexUnlocked)
				{
					m_pointIndex--;
				}

				if (m_pointIndex > m_maxPoint)
					m_pointIndex = m_maxPoint;
				
				// When last WorldPoint is reached
				if (m_pointIndex == m_maxPoint)
				{
					int nextWorld = m_worldIndex + 1;
					if (nextWorld < m_worlds.Length)
					{
						// Change to WorldOverviewMap and set pointIndex to the current world we left
						ChangeToWorld(nextWorld);
                    }
					else
					{
						Debug.Log("Last WorldPoint of last World reached!");
					}
				}

				ResetTimers();
			}
		}
		
		if (m_pointIndex == 0 && Input.GetKeyDown(KeyCode.Backspace))
		{
			// Change to prev World
			int prevWorld = m_worldIndex - 1;
			if (prevWorld >= 0)
			{
				ChangeToWorld(prevWorld, m_worlds[prevWorld].worldPoints.Length - 2); // 1 before last worldpoint (because last worldPoint is the exit)
			}
		}

		float prevMoveTimer = m_moveTimer;

		if (m_moveTimer < m_timeBetweenPoints)
			m_moveTimer += Time.deltaTime;
		if (m_moveTimer > m_timeBetweenPoints)
			m_moveTimer = m_timeBetweenPoints;

		// If Movement has finished
		if (m_moveTimer != prevMoveTimer && m_moveTimer == m_timeBetweenPoints)
		{
			OnMovementFinished();
		}

		// Update PlayerMark Position
		m_playerMark.position = Vector3.Lerp(m_worlds[m_worldIndex].worldPoints[m_prevPoint].position,
			m_worlds[m_worldIndex].worldPoints[m_pointIndex].position,
			m_moveTimer / m_timeBetweenPoints);

		// Update SaveGame
		SaveGame.GetData().worldMapWorldIndex = m_worldIndex;
		SaveGame.GetData().worldMapPointIndex = m_pointIndex;
	}

	void ResetTimers()
	{
		m_inputTimer = m_inputInterval;
		m_moveTimer = 0.0f;
	}

	Vector2 GetInputVector()
	{
		Vector2 input = Vector2.zero;
		input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (!Singleton.GameVarsManager.m_gameVars.m_keyboardControl)
		{
			XInput.XInputState state;
			XInput.XInputGetState(0, out state);
			input = new Vector2(state.Gamepad.LeftThumbX / 32768.0f, state.Gamepad.LeftThumbY / 32768.0f);
		}
		input.Normalize();
		return input;
	}

	void CalcVectorDiff(Vector2 input, out float diffLower, out float diffHigher)
	{
		m_prevPoint = m_pointIndex;

		Vector2 prevPos = m_worlds[m_worldIndex].worldPoints[m_prevPoint].position;
		Vector2 lowerIndexPos = m_worlds[m_worldIndex].worldPoints[(m_pointIndex - 1 < 0) ? 0 : (m_pointIndex - 1)].position;
		Vector2 higherIndexPos = m_worlds[m_worldIndex].worldPoints[(m_pointIndex + 1 > m_maxPoint) ? m_maxPoint : (m_pointIndex + 1)].position;

		Vector2 toLower = (lowerIndexPos - prevPos).normalized;
		Vector2 toHigher = (higherIndexPos - prevPos).normalized;
		Vector2 toInput = input.normalized;

		float dotLower = Vector2.Dot(toLower, toInput);
		float dotHigher = Vector2.Dot(toHigher, toInput);

		diffLower = Mathf.Abs(1.0f - dotLower);
		diffHigher = Mathf.Abs(1.0f - dotHigher);

		//if (input.x != 0.0f || input.y != 0.0f)
		//	Debug.Log(dotLower + " | " + dotHigher + " | diffLower: " + diffLower + " | diffHigher: " + diffHigher);
	}

	void HandleInputSelectLevel()
	{
		if (m_moveTimer != m_timeBetweenPoints)
			return;

		bool inputSelect = false;
		inputSelect = Input.GetKey(KeyCode.Space);
		if (!Singleton.GameVarsManager.m_gameVars.m_keyboardControl)
		{
			XInput.XInputState state;
			XInput.XInputGetState(0, out state);
			inputSelect |= state.Gamepad.IsButtonDown(XInput.XInputButtons.A);
		}

		if (inputSelect)
		{
			SaveGame.GetData().worldMapWorldIndex = m_worldIndex;
			SaveGame.GetData().worldMapPointIndex = m_pointIndex;

			// Auto Save
			SaveGame.Save();

			m_canvas.SetActive(false);
            Camera.main.GetComponent<CameraIntoBook>().StartCameraTransition(GetCurrentLevelIndex());
		}
	}

	int GetCurrentLevelIndex()
	{
		// Sum up all WorldPoints from previous Worlds
		int sumWorldPoints = 0;
		for (int i = 0; i < m_worldIndex; i++)
		{
			sumWorldPoints += m_worlds[i].worldPoints.Length - 1; // - 1 because the last WorldPoint does not count (it is used to change to the next world)
		}

		return (sumWorldPoints + m_pointIndex);
	}

	private void ShowLevelInfo()
	{
		int levelIndex = GetCurrentLevelIndex();
        m_Collectables.text = SaveGame.GetNumRewardsCollected(levelIndex) + " of " + GameConstants.REWARDS_PER_LEVEL;
		m_BestTime.text = SaveGame.GetData().bestTimes[levelIndex] + " seconds";
		m_TryCount.text = SaveGame.GetData().numTries[levelIndex] + "x";
	}

	private void OnMovementFinished()
	{
		// Debug.Log("New Left Page Texture Index [worldIndex = " + m_worldIndex + "] : " + GetTextureIndexLeftPage());
		ChangePageTexture(m_book.leftPage, m_worldPages[m_worldIndex].texturesLeftPage[GetTextureIndexLeftPage()]);

		// Debug.Log("New Right Page Texture Index [worldIndex = " + m_worldIndex + "] : " + GetTextureIndexRightPage());
		ChangePageTexture(m_book.rightPage, m_worldPages[m_worldIndex].texturesRightPage[GetTextureIndexRightPage()]);
	}

	private void OnPageTurnEnd()
	{
		// Before the Anim Page will be invisible, apply Textures
		if (m_bookAnim.turnLeft)
		{
			ChangePageTexture(m_book.rightPage, m_bookAnim.nextRightTex);
		}
		else
		{
			ChangePageTexture(m_book.leftPage, m_bookAnim.nextLeftTex);
		}

		SetAnimPageVisible(false);

		m_playerMark.gameObject.SetActive(true);
		InitWorld();
	}

	private void SetAnimPageVisible(bool visible)
	{
		m_book.animFrontPage.SetActive(visible);
		m_book.animBackPage.SetActive(visible);
	}

	private void TurnPage(bool turnLeft, Texture2D nextLeftTex, Texture2D nextRightTex)
	{
		// Setup Book Animation
		m_bookAnim.turnLeft = turnLeft;
        m_bookAnim.timer = BookAnimation.duration;
		m_bookAnim.nextLeftTex = nextLeftTex;
		m_bookAnim.nextRightTex = nextRightTex;

		// Change Textures
		if (turnLeft)
		{
			ChangePageTexture(m_book.animBackPage, GetPageTexture(m_book.leftPage));
			ChangePageTexture(m_book.animFrontPage, nextRightTex);
			ChangePageTexture(m_book.leftPage, nextLeftTex);
		}
		else
		{
			ChangePageTexture(m_book.animBackPage, nextLeftTex);
			ChangePageTexture(m_book.animFrontPage, GetPageTexture(m_book.rightPage));
			ChangePageTexture(m_book.rightPage, nextRightTex);
		}


		// Show Animation Page
		SetAnimPageVisible(true);

		// Play Animation
		if (turnLeft)
			m_book.animator.SetTrigger("turnLeft");
		else
			m_book.animator.SetTrigger("turnRight");
	}

	private void ChangePageTexture(GameObject pageObj, Texture2D texture)
	{
		pageObj.GetComponent<Renderer>().material.mainTexture = texture;
	}

	private Texture2D GetPageTexture(GameObject pageObj)
	{
		return (Texture2D)pageObj.GetComponent<Renderer>().material.mainTexture;
    }

	private int GetTextureIndexLeftPage()
	{
		int textureIndex = 0;

		int numPrevWorldPoints = GetCurrentLevelIndex() - m_pointIndex - 1;
		// Add Unlocked Levels
		textureIndex += SaveGame.GetData().levelIndexUnlocked - 1 - numPrevWorldPoints;
		// Limit
		int maxPointIndex = m_worlds[m_worldIndex].worldPoints.Length - 2;
		textureIndex = Mathf.Clamp(textureIndex, 0, maxPointIndex);

		return textureIndex;
	}

	private int GetTextureIndexRightPage()
	{
		return m_pointIndex;
	}

	public void OnPageLeftButtonClicked()
	{
		Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

		// Change to prev World
		int prevWorld = m_worldIndex - 1;
		if (prevWorld >= 0)
		{
			ChangeToWorld(prevWorld, m_worlds[prevWorld].worldPoints.Length - 2); // 1 before last worldpoint (because last worldPoint is the exit)
		}
	}

	public void OnPageRightButtonClicked()
	{
		Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

		
		m_worldIndex++;
		int tempPointIndex = m_pointIndex;
		m_pointIndex = 0;
		// Move back if level is not unlocked
		if (GetCurrentLevelIndex() > SaveGame.GetData().levelIndexUnlocked)
		{
			m_worldIndex--;
			m_pointIndex = tempPointIndex;
        }
		else
		{
			m_worldIndex--;
			// Change to next World
			int nextWorld = m_worldIndex + 1;
			if (nextWorld < m_worlds.Length)
			{
				ChangeToWorld(nextWorld);
			}
		}

	}

	public void OnBackToMainMenuButtonClicked()
	{
		Singleton.AudioManager.PlaySFX(SFXName.menuButtonClick);

		SaveGame.Save();

		// Go back to Main Menu Scene
		Application.LoadLevel(0);
	}

	public void OnPointerExit()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}
}
