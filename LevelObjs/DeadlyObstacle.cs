using UnityEngine;
using System.Collections;

public class DeadlyObstacle : MonoBehaviour
{
	public bool m_isActive = true;
	public bool m_doNotChangeState = false;
	public bool m_destroyFallingObstacle = true;
	public bool m_shouldReactivate = false;
	public float m_reactivationTime = 3.0f;
	public bool m_destructableByAttack = false;
	public SFXName m_soundOnActivation;

	private float m_timerReactivation = 0.0f;

	void Start()
	{

	}

	void Update()
	{
		if(!m_doNotChangeState && m_shouldReactivate && m_timerReactivation > 0.0f)
		{
			m_timerReactivation -= Time.deltaTime;
			if(m_timerReactivation <= 0.0f)
			{
				m_isActive = true;
            }
        }
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(m_destructableByAttack && other.tag == StringManager.Tags.basicAttack)
		{
			Destroy(this.gameObject);
			return;
        }
		if (!m_isActive)
		{
			return;
		}
		if (other.tag == StringManager.Tags.player ||
			other.tag == StringManager.Tags.redRidingHood)
		{
			other.GetComponent<BaseMover>().OnDeath();
			Reactivate();
        }
		else if(other.tag == StringManager.Tags.fallingObstacle)
		{
			if (m_destroyFallingObstacle)
			{
				Destroy(other.gameObject);
				Reactivate();
            }
		}
	}

	void Reactivate()
	{
		Singleton.AudioManager.PlaySFX(m_soundOnActivation);
		if (!m_doNotChangeState)
		{
			m_isActive = false;
		}
        if (m_shouldReactivate && !m_doNotChangeState)
		{
			m_timerReactivation = m_reactivationTime;
		}
	}
}
