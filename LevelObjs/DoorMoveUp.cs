using UnityEngine;
using System.Collections;

public class DoorMoveUp : Door
{
	public float m_animDuration = 1.0f;
	public Vector3 m_targetPos;
	private Vector3 m_startPos;

	private float m_timerAnim;
	private bool m_opening = true;

	void Start()
	{
		m_startPos = m_doorObjs[0].localPosition;
		m_timerAnim = m_animDuration;
    }

	void Update()
	{
		if (m_timerAnim < m_animDuration)
		{
			m_timerAnim += Time.deltaTime;

			if(m_opening)
			{
				m_doorObjs[0].localPosition = Vector3.Lerp(m_startPos, m_targetPos, m_timerAnim / m_animDuration);
			}
			else
			{
				m_doorObjs[0].localPosition = Vector3.Lerp(m_targetPos, m_startPos, m_timerAnim / m_animDuration);
			}
		}
    }

	public override void Open()
	{
		base.Open();

		m_timerAnim = 0.0f;
		m_opening = true;

		Singleton.AudioManager.PlaySFX(SFXName.doorBigOpen);
    }

	public override void Close()
	{
		base.Close();

		m_timerAnim = 0.0f;
		m_opening = false;

		Singleton.AudioManager.PlaySFX(SFXName.doorBigClose);
	}
}
