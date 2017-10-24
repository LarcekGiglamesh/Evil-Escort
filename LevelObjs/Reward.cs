using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
	public int m_rewardIndex = -1;
	private Text m_text;

	void Start()
	{
		if (m_rewardIndex == -1)
		{
			Debug.LogError("The reward index was not set for this instance!"
				+ " WorldIndex: " + SaveGame.GetData().worldMapWorldIndex
				+ " PointIndex: " + SaveGame.GetData().worldMapPointIndex);
		}

		// If it is already unlocked
		if (SaveGame.GetData().rewardsUnlocked[m_rewardIndex])
		{
			Destroy(this.gameObject);
		}

		GameObject textObj = GameObject.Find(StringManager.Names.collectableCounter);
		if (textObj != null)
		{
			m_text = textObj.GetComponent<Text>();
		}
		UpdateRewardsUI();
    }

	void Update()
	{
		if (m_text == null)
		{
			GameObject textObj = GameObject.Find(StringManager.Names.collectableCounter);
			if (textObj != null)
			{
				m_text = textObj.GetComponent<Text>();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player)
		{
			// Unlock Reward
			SaveGame.GetData().rewardsUnlocked[m_rewardIndex] = true;
			StatsTracker.m_numRewardsCollected++;

			UpdateRewardsUI();

			Destroy(this.gameObject);
		}
	}

	private void UpdateRewardsUI()
	{
		if (m_text != null)
		{
			int numRewards = SaveGame.GetNumRewardsCollected(SaveGame.GetCurrentLevelIndex());
			m_text.text = numRewards + " / " + GameConstants.REWARDS_PER_LEVEL;
		}
	}
}
