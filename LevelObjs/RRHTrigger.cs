using UnityEngine;
using System.Collections;

public class RRHTrigger : MonoBehaviour
{
	public float m_timeWaitBeforeWalk = 1.0f;

	private GameObject m_rrhObj;
	private Animator m_rrhAnimatorIdle;
	private Animator m_rrhAnimatorNormal;
	private PlayerMovement m_playerMovement;

	private float m_timeEntered = 0.0f;

	void Start()
	{
		m_rrhObj = GameObject.FindGameObjectWithTag(StringManager.Tags.redRidingHood);
		Animator[] animators = m_rrhObj.GetComponentsInChildren<Animator>(true);
		m_rrhAnimatorNormal = animators[0];
		m_rrhAnimatorIdle = animators[1];

		GameObject playerObj = GameObject.FindGameObjectWithTag(StringManager.Tags.player);
		m_playerMovement = playerObj.GetComponent<PlayerMovement>();
    }

	void Update()
	{
		if (m_timeEntered != 0.0f && Time.time - m_timeEntered > m_timeWaitBeforeWalk)
		{
			m_rrhAnimatorIdle.gameObject.SetActive(false);
			m_rrhAnimatorNormal.gameObject.SetActive(true);
			m_rrhAnimatorNormal.transform.localEulerAngles = new Vector3(0, 90, 0);
			m_rrhObj.GetComponent<RedRidingHood>().enabled = true;

			m_playerMovement.m_movementEnabled = true;
			m_playerMovement.GetComponent<PlayerAttack>().enabled = true;

			Singleton.IngameMenu.displayUserInterface();

			m_timeEntered = 0.0f;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player)
		{
			m_timeEntered = Time.time;

			m_playerMovement.m_movementEnabled = false;
			m_playerMovement.GetComponent<PlayerAttack>().enabled = false;

			m_playerMovement.GetComponentInChildren<Animator>().SetBool("isWalking", false);


		}
	}
	
}
