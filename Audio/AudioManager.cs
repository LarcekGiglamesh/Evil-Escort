using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BGMName
{
	NONE,
	mainMenu,
	ingame,
	deathScreen,
	teamLogo,
}

public enum SFXName
{
	NONE,
	bearTrap,
	menuButtonClick,
	wolfSteps,
	playerStepRight,
    playerTouchGround,
	zirp,
	birds,
	doorSmallOpen,
	doorSmallClose,
	doorBigOpen,
	doorBigClose,
}

[System.Serializable]
// class/struct which contains information about one single audioClip
public class SFXAudioObj
{
	public SFXName m_name; // name of the audio-clip, which is defined in AudioCollection enum
	public List<AudioClip> m_clips; // the real audio file(s), .wav / .mp3 / whatever
	public float m_volume = 1.0f; // volume of the sound
	public bool m_loop = false; // loop sound or not?
}

[System.Serializable]
// class/struct which contains information about one single audioClip
public class BGMAudioObj
{
	public BGMName m_name; // name of the audio-clip, which is defined in AudioCollection enum
	public List<AudioClip> m_clips; // the real audio file(s), .wav / .mp3 / whatever
	public float m_volume = 1.0f; // volume of the sound
}

public class AudioManager : MonoBehaviour
{
	public SFXAudioObj[] m_sfxAudioObjs; // index = AudioObj.m_name => SFXName
	public BGMAudioObj[] m_bgmAudioObjs; // index = AudioObj.m_name => BGMName

	public float m_globalAudioVolume = 1.0f;
	public float m_globalSFXVolume = 1.0f;
	public float m_globalBGMVolume = 1.0f;

	private AudioSource m_backgroundAudioSource = null;

	private List<AudioSource> m_currentAudioSources = new List<AudioSource>();

	private BGMAudioObj m_currentBGMAudioObj;

	void Start()
	{
		m_backgroundAudioSource = GetComponent<AudioSource>();
		AudioListener.volume = m_globalAudioVolume;

#if DEBUG
		CheckAudioObjs();
#endif
	}

	private void CheckAudioObjs()
	{
		// Check which AudioObjs have 0 clips
		for (int i = 0; i < m_sfxAudioObjs.Length; i++)
		{
			if (m_sfxAudioObjs[i].m_name == SFXName.NONE)
				continue;

			if (m_sfxAudioObjs[i].m_clips.Count == 0)
			{
				Debug.LogError("No Clips in SFX: " + m_sfxAudioObjs[i].m_name.ToString());
			}
			else
			{
				// is there at least one audioClip null?
				for (int j = 0; j < m_sfxAudioObjs[i].m_clips.Count; j++)
				{
					if (m_sfxAudioObjs[i].m_clips[j] == null)
					{
						Debug.LogError("Missing Clip(s) in SFX: " + m_sfxAudioObjs[i].m_name.ToString());
						break;
					}
				}
			}
		}
		for (int i = 0; i < m_bgmAudioObjs.Length; i++)
		{
			if (m_bgmAudioObjs[i].m_name == BGMName.NONE)
				continue;

			if (m_bgmAudioObjs[i].m_clips.Count == 0)
			{
				Debug.LogError("No Clips in BGM: " + m_bgmAudioObjs[i].m_name.ToString());
			}
			else
			{
				// is there at least one audioClip null?
				for (int j = 0; j < m_bgmAudioObjs[i].m_clips.Count; j++)
				{
					if (m_bgmAudioObjs[i].m_clips[j] == null)
					{
						Debug.LogError("Missing Clip(s) in BGM: " + m_bgmAudioObjs[i].m_name.ToString());
						break;
					}
				}
			}
		}
	}
	
	void Update ()
	{
		DestroyFinishedAudioSources();
	}

	private void DestroyFinishedAudioSources()
	{
		int numAudioSources = m_currentAudioSources.Count;
		for (int i = 0; i < numAudioSources; i++)
		{
			if(m_currentAudioSources[i].isPlaying == false)
			{
				// Is Loop Sound?
				if(m_currentAudioSources[i].loop == false)
				{
					// Destroy audio source
					Destroy(m_currentAudioSources[i]);
					m_currentAudioSources.RemoveAt(i);
					numAudioSources--;
					i--;
				}
			}
		}
	}

	public AudioSource PlaySFX(SFXName sfxName, float timeDelay = 0.0f, int index = -1, bool playImmediate = true)
	{
		if (sfxName == SFXName.NONE)
			return null;

		SFXAudioObj audioObj = m_sfxAudioObjs[(int)sfxName];
		if (audioObj.m_clips.Count == 0)
		{
			Debug.LogError("No Clips in SFX: " + sfxName.ToString());
			return null;
		}
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		if(index == -1)
		{
			// Random AudioClip
			audioSource.clip = audioObj.m_clips[Random.Range(0, audioObj.m_clips.Count - 1)];
		}
		else
		{
			// AudioClip By Index
			audioSource.clip = audioObj.m_clips[index];
		}
		audioSource.volume = audioObj.m_volume * m_globalSFXVolume;
		audioSource.loop = audioObj.m_loop;
		if (playImmediate)
		{
			audioSource.PlayDelayed(timeDelay);
		}
		m_currentAudioSources.Add(audioSource);
		return audioSource;
	}

	public void PlayBGM(BGMName bgmName, float timeDelay = 0.0f)
	{
		if (bgmName == BGMName.NONE)
			return;

		BGMAudioObj audioObj = m_bgmAudioObjs[(int)bgmName];
		m_currentBGMAudioObj = audioObj;
		if(m_backgroundAudioSource == null)
		{
			m_backgroundAudioSource = GetComponent<AudioSource>();
		}
		m_backgroundAudioSource.loop = true;
        m_backgroundAudioSource.clip = audioObj.m_clips[Random.Range(0, audioObj.m_clips.Count - 1)];
		m_backgroundAudioSource.volume = audioObj.m_volume * m_globalBGMVolume;
		m_backgroundAudioSource.PlayDelayed(timeDelay);
	}

	public void StopSFX(AudioSource audioSource)
	{
		if(audioSource != null)
		{
			Destroy(audioSource);
			if(m_currentAudioSources.Contains(audioSource))
			{
				m_currentAudioSources.Remove(audioSource);
			}
		}
	}

	public void StopBGM()
	{
		m_backgroundAudioSource.volume = 0.0f;
        m_backgroundAudioSource.Stop();
    }

	public SFXAudioObj GetSFXAudioObj(SFXName sfxName)
	{
		return m_sfxAudioObjs[(int)sfxName];
	}

	public void ClearSFXList()
	{
		for (int i = 0; i < m_currentAudioSources.Count; i++ )
		{
			Destroy(m_currentAudioSources[i]);
		}
		m_currentAudioSources.Clear();
	}

	public void IncreaseGlobalAudio(float volume)
	{
		m_globalAudioVolume += volume;
		if (m_globalAudioVolume > 1.0f)
			m_globalAudioVolume = 1.0f;

		AudioListener.volume = m_globalAudioVolume;
	}

	public void DecreaseGlobalAudio(float volume)
	{
		m_globalAudioVolume -= volume;
		if (m_globalAudioVolume < 0.0f)
			m_globalAudioVolume = 0.0f;

		AudioListener.volume = m_globalAudioVolume;
	}

	public void IncreaseGlobalSFXAudio(float volume)
	{
		m_globalSFXVolume += volume;
		if (m_globalSFXVolume > 1.0f)
			m_globalSFXVolume = 1.0f;
	}

	public void DecreaseGlobalSFXAudio(float volume)
	{
		m_globalSFXVolume -= volume;
		if (m_globalSFXVolume < 0.0f)
			m_globalSFXVolume = 0.0f;
	}

	public void IncreaseGlobalBGMAudio(float volume)
	{
		m_globalBGMVolume += volume;
		if (m_globalBGMVolume > 1.0f)
			m_globalBGMVolume = 1.0f;

		m_backgroundAudioSource.volume = m_globalBGMVolume;
	}

	public void DecreaseGlobalBGMAudio(float volume)
	{
		m_globalBGMVolume -= volume;
		if (m_globalBGMVolume < 0.0f)
			m_globalBGMVolume = 0.0f;

		m_backgroundAudioSource.volume = m_globalBGMVolume;
	}

	public void SetGlobalAudioVolume(float volume)
	{
		m_globalAudioVolume = volume;
		AudioListener.volume = m_globalAudioVolume;
	}

	public void SetGlobalSFXAudioVolume(float volume)
	{
		m_globalSFXVolume = volume;
	}

	public void SetGlobalBGMAudioVolume(float volume)
	{
		m_globalBGMVolume = volume;
		m_backgroundAudioSource.volume = m_currentBGMAudioObj.m_volume * m_globalBGMVolume;
	}
}
