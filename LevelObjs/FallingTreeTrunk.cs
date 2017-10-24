using UnityEngine;
using System.Collections;

public class FallingTreeTrunk : MonoBehaviour {

	private float m_HorizontalDuration = 4.0f / 24.44f;
	private float m_VerticalDuration = 4.0f / 24.44f;
	private float m_EndDuration = 0.917f;
	public Vector2 m_Velocity = new Vector2(4.0f, -16.0f);
	
	public bool m_TestAnimation = false;
	private bool m_DoAnimation = false;
	private float m_Timer = 0.0f;
	private Vector3 pos;
	private bool m_destroyMe;

	public void StartAnimation(bool destroyMe, float facingDir)
	{
		m_DoAnimation = true;
		m_Timer = Time.time;
		m_destroyMe = destroyMe;

		Vector3 scale = transform.localScale;
		scale.y = facingDir;
		transform.localScale = scale;

		GetComponentInChildren<Animator>().SetTrigger("Falling");
	}

	// Update is called once per frame
	void Update () {
		if (m_TestAnimation)
		{
			StartAnimation(true, 1.0f);
			m_TestAnimation = false;
		}

		if (m_DoAnimation)
		{
			if(Time.time - m_Timer <= m_HorizontalDuration)
			{
				pos = transform.position;
				transform.position = new Vector3(pos.x + m_Velocity.x * Time.deltaTime, pos.y, pos.z);
			}

			if (Time.time - m_Timer > m_VerticalDuration)
			{
				pos = transform.position;
				transform.position = new Vector3(pos.x, pos.y + m_Velocity.y * Time.deltaTime, pos.z);
				Debug.Log(transform.position);
			}

			if (Time.time - m_Timer > m_EndDuration)
			{
				m_DoAnimation = false;
				if (m_destroyMe)
					Destroy(this.gameObject);
			}

		}
	}
}
