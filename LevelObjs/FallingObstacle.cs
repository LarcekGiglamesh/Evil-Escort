using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Controller2D))]
public class FallingObstacle : MonoBehaviour
{
	public float m_speed = 10.0f;
	public float m_timeUntilFalling = 1.0f;
	public bool m_fallOnTouchAbove = true;

	private bool m_isFalling = false;
	private bool m_timerStarted = false;
	private float m_timer = 0.0f;

	private Controller2D m_controller2D;

	void Start()
	{
		m_controller2D = GetComponent<Controller2D>();
    }

	void Update()
	{
		if(m_timerStarted && m_timer > 0.0f)
		{
			m_timer -= Time.deltaTime;
			if(m_timer <= 0.0f)
			{
				m_isFalling = true;
				m_timer = 0.0f;
            }
		}
		if(m_isFalling)
		{
			m_controller2D.Move(new Vector2(0.0f, -1.0f * m_speed * Time.deltaTime), false);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(m_fallOnTouchAbove)
		{
			if (other.tag == StringManager.Tags.player ||
				other.tag == StringManager.Tags.redRidingHood ||
				other.tag == StringManager.Tags.pickupObj)
			{
				if (other.transform.position.y > this.transform.position.y + transform.localScale.y)
				{
					StartTimer();
				}
			}
		}
    }

	public void StartTimer()
	{
		if (m_timerStarted == false)
		{
			m_timerStarted = true;
			m_timer = m_timeUntilFalling;
		}
	}

	public void SetIsFalling(bool val)
	{
		m_isFalling = val;
	}
}
