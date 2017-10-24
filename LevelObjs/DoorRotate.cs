using UnityEngine;
using System.Collections;

public class DoorRotate : Door
{
	[Range(0.0f, 5.0f)]
	public float m_rotateDuration = 1.0f;
	public float m_leftObjEulerY = -90.0f;
	public float m_rightObjEulerY = 90.0f;

	private BoxCollider2D m_collider;

	private float m_timerRotation = -1.0f;
	private Quaternion m_leftTargetRotation;
	private Quaternion m_rightTargetRotation;


	void Start()
	{
		m_collider = GetComponentInChildren<BoxCollider2D>();
    }

	void Update()
	{
		if (m_timerRotation >= 0.0f)
		{
			m_timerRotation += Time.deltaTime;
			m_timerRotation = Mathf.Clamp(m_timerRotation, 0.0f, 1.0f);

			m_doorObjs[0].localRotation = Quaternion.Slerp(m_doorObjs[0].localRotation, m_leftTargetRotation, m_timerRotation / m_rotateDuration);
			m_doorObjs[1].localRotation = Quaternion.Slerp(m_doorObjs[1].localRotation, m_rightTargetRotation, m_timerRotation / m_rotateDuration);

			if (m_timerRotation >= m_rotateDuration)
			{
				// Stop Timer
				m_timerRotation = -1.0f;
			}
		}
	}

	public override void Open()
	{
		base.Open();

		Singleton.AudioManager.PlaySFX(SFXName.doorSmallOpen);

		// Start Timer
		m_timerRotation = 0.0f;
		
		m_leftTargetRotation = Quaternion.Euler(m_doorObjs[0].eulerAngles.x, m_leftObjEulerY, m_doorObjs[0].eulerAngles.z);
		m_rightTargetRotation = Quaternion.Euler(m_doorObjs[1].eulerAngles.x, m_rightObjEulerY, m_doorObjs[1].eulerAngles.z);

		//m_doorObjs[0].localRotation = Quaternion.Euler(m_doorObjs[0].eulerAngles.x, m_leftObjEulerY, m_doorObjs[0].eulerAngles.z);
		//m_doorObjs[1].localRotation = Quaternion.Euler(m_doorObjs[1].eulerAngles.x, m_rightObjEulerY, m_doorObjs[1].eulerAngles.z);

		m_collider.enabled = false;

	}

	public override void Close()
	{
		base.Close();

		Singleton.AudioManager.PlaySFX(SFXName.doorSmallClose);

		// Start Timer
		m_timerRotation = 0.0f;

		m_leftTargetRotation = Quaternion.Euler(m_doorObjs[0].eulerAngles.x, 0.0f, m_doorObjs[0].eulerAngles.z);
		m_rightTargetRotation = Quaternion.Euler(m_doorObjs[1].eulerAngles.x, 0.0f, m_doorObjs[1].eulerAngles.z);

		//m_doorObjs[0].localRotation = Quaternion.Euler(m_doorObjs[0].eulerAngles.x, 0.0f, m_doorObjs[0].eulerAngles.z);
		//m_doorObjs[1].localRotation = Quaternion.Euler(m_doorObjs[1].eulerAngles.x, 0.0f, m_doorObjs[1].eulerAngles.z);

		m_collider.enabled = true;
	}

}
