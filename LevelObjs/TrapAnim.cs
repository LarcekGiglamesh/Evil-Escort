using UnityEngine;
using System.Collections;

public class TrapAnim : MonoBehaviour {

	public bool m_isActive = true;
	private Animator m_Animator;

	void Start()
	{
		m_Animator = GetComponent<Animator>();
	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{

		if (!m_isActive)
		{
			return;
		}

		if (other.tag.Equals(StringManager.Tags.player)
			|| other.tag.Equals(StringManager.Tags.redRidingHood)
			|| other.tag.Equals(StringManager.Tags.fallingObstacle))
		{
			m_isActive = false;

			// m_Animator.SetTrigger("doAnim");
			m_Animator.SetBool("playAnim", true);
		}
	}

}
