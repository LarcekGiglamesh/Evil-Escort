using UnityEngine;
using System.Collections;

public class TrapAnimation : MonoBehaviour
{

	public float m_Angle = 70.0f;

	public bool m_isActive = true;
	private bool m_doNotChangeState = true;
	public bool m_shouldReactivate = false;
	[Tooltip("Time in Seconds until Reactivation")]
	public float m_timerReactivation = 5.0f;
	private float m_timerReset;

	public GameObject m_LeftSide;
	public GameObject m_RightSide;


	void Start()
	{
		m_timerReset = m_timerReactivation;
	}

	void Update()
	{
		if (!m_doNotChangeState && m_shouldReactivate && m_timerReactivation > 0.0f)
		{
			m_timerReactivation -= Time.deltaTime;
			if (m_timerReactivation <= 0.0f)
			{
				float angle = Mathf.Abs(m_Angle);
				RotateTrapPart(m_LeftSide, angle);
				RotateTrapPart(m_RightSide, angle);     //TODO: wenn rechtes Model gefixt, vorzeichen drehen

				m_timerReactivation = m_timerReset;
				m_doNotChangeState = true;

				m_isActive = true;
			}
		}
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
			float angle = Mathf.Abs(m_Angle);
			RotateTrapPart(m_LeftSide, -angle);
			RotateTrapPart(m_RightSide, -angle);		//TODO: wenn rechtes Model gefixt, vorzeichen drehen

			m_isActive = false;

			m_doNotChangeState = !m_shouldReactivate;
		}
	}

	void RotateTrapPart(GameObject side, float angle)
	{
		side.transform.Rotate(0.0f, 0.0f, angle);
	}
	
}
