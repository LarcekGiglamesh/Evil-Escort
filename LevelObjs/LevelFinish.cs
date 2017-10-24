using UnityEngine;
using System.Collections;

public class LevelFinish : MonoBehaviour
{
	private bool m_redRidingHoodHasEntered = false;

	void Start()
	{

    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.redRidingHood)
		{
			Camera.main.GetComponent<FollowPlayerCamera>().m_ignoreRRH = true;
			m_redRidingHoodHasEntered = true;
			Destroy(other.gameObject);
		}

		if (m_redRidingHoodHasEntered && other.tag == StringManager.Tags.player)
		{
			StatsTracker.OnLevelSuccess();
			Destroy(other.gameObject);
		}
	}

}
