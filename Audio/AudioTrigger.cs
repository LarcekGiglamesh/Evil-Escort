using UnityEngine;
using System.Collections;

public class AudioTrigger : MonoBehaviour
{
	public SFXName m_sfxName;
	public float m_playTimeDelay;

	private AudioSource m_audioSource;

	void Start()
	{
		
	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player)
		{
			if (m_audioSource == null)
			{
				m_audioSource = Singleton.AudioManager.PlaySFX(m_sfxName, m_playTimeDelay);
				Debug.Log("Start playing!");
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		Singleton.AudioManager.StopSFX(m_audioSource);
		Debug.Log("Stop playing!");
	}
}
