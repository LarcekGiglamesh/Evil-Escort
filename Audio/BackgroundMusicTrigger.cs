using UnityEngine;
using System.Collections;

public class BackgroundMusicTrigger : MonoBehaviour
{
	// Choice of GD
	public BGMName m_LevelType = BGMName.NONE;
	// Remember Last Music
	private BGMName m_LevelBgmLastState = BGMName.NONE;

	// Use this for initialization
	void Start ()
	{
		m_LevelBgmLastState = m_LevelType;
        Singleton.AudioManager.PlayBGM(m_LevelType, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (m_LevelBgmLastState != m_LevelType)
		{
			m_LevelBgmLastState = m_LevelType;
			Singleton.AudioManager.StopBGM();
			Singleton.AudioManager.PlayBGM(m_LevelType, 0.0f);
		}
	}

	// Change Music when necessary (eg Player Death)
	public void ChangeBackgroundMusic(BGMName nextMusic)
	{
		m_LevelType = nextMusic;
	}
}
