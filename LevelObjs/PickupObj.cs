using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class PickupObj : BaseMover
{
	public float m_speedDecFactor = 4.0f;
	public float m_maxSpeed = 4.0f;

	public Vector2 m_moveDir = Vector2.zero;

	override protected void Start()
	{
		m_speed = m_maxSpeed;
		m_controller2D = GetComponent<Controller2D>();
    }

	override protected void Update()
	{
		Vector2 vMove = m_moveDir * m_speed;
		vMove += m_force;
		vMove *= Time.deltaTime;
		m_controller2D.Move(vMove);

		m_speed -= Time.deltaTime * m_speedDecFactor;
		if (m_speed < 0.0f)
		{
			m_speed = 0.0f;
			m_moveDir = Vector2.zero;
		}

		m_force.y += m_gravity * Time.deltaTime;
		if (m_controller2D.collisions.below)
		{
			m_force = Vector2.zero;
        }
    }

	public Vector2 GetMoveDir()
	{
		return m_moveDir;
	}

	public void SetMoveDir(Vector2 moveDir)
	{
		if(moveDir == Vector2.zero)
		{
			m_moveDir = Vector2.zero;
			m_speed = 0.0f;
			return;
		}
		m_moveDir = moveDir.normalized;
		m_speed = m_maxSpeed;
    }
}
