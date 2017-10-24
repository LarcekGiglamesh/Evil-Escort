using UnityEngine;
using System.Collections;

public class Singleton
{
	private static Transform m_gameSystemsTransform = null;
	private static GameVarsManager m_gameVarsManager = null;
	private static AudioManager m_audioManager = null;
	private static EnableIngameMenu m_IngameMenu = null;

	public static Transform GameSystemsTransform
	{
		get
		{
			if(m_gameSystemsTransform == null)
			{
				m_gameSystemsTransform = GameObject.Find(StringManager.Singleton.gameSystems).transform;
			}
			return m_gameSystemsTransform;
		}
	}

	public static GameVarsManager GameVarsManager
	{
		get
		{
			if (m_gameVarsManager == null)
			{
				m_gameVarsManager = GameObject.Find(StringManager.Singleton.gameVarsManager).GetComponent<GameVarsManager>();
			}
			return m_gameVarsManager;
		}
	}

	public static AudioManager AudioManager
	{
		get
		{
			if (m_audioManager == null)
			{
				m_audioManager = GameObject.Find(StringManager.Singleton.audioManager).GetComponent<AudioManager>();
			}
			return m_audioManager;
		}
	}

	public static EnableIngameMenu IngameMenu
	{
		get
		{
			if (m_IngameMenu == null)
			{
				m_IngameMenu = GameObject.Find(StringManager.Singleton.ingameMenu_Canvas).GetComponent<EnableIngameMenu>();
			}
			return m_IngameMenu;
		}
	}
}
