using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class BaseMover : MonoBehaviour
{
	public float m_speed;
	public float m_gravity;
	public float m_jumpVelocity;
	public float m_accelerationTimeAirborne;
	public float m_accelerationTimeGrounded;
	public float m_forceDecPerSec = 5.0f;

	protected Controller2D m_controller2D;
	protected Vector2 m_velocity;
	protected Vector2 m_force = Vector2.zero;
	protected bool m_jumpControlEnabled = true;
	
	protected virtual void Start()
	{
		m_controller2D = GetComponent<Controller2D>();
	}

	protected virtual void Update()
	{
		if (m_controller2D.collisions.above || m_controller2D.collisions.below)
		{
			m_velocity.y = 0.0f;
			m_force = Vector2.zero;
		}

		if (m_force != Vector2.zero)
		{
			if(m_force.x > 0.0f)
			{
				m_force.x -= m_forceDecPerSec * Time.deltaTime;
				if (m_force.x < 0.0f)
					m_force.x = 0.0f;
			}
			if(m_force.y > 0.0f)
			{
				m_force.y -= m_forceDecPerSec * Time.deltaTime;
				if (m_force.y < 0.0f)
					m_force.y = 0.0f;
			}
		}
	}

	public void AddForce(Vector2 force)
	{
		m_force += force;
	}

	public void SetForce(Vector2 force)
	{
		m_force = force;
	}

	public virtual void SetVelocity(Vector2 velocity)
	{
		m_velocity = velocity;
	}

	public virtual void OnDeath()
	{

	}

	public virtual void OnDamaged(int damage)
	{

	}

	public void SetJumpControlEnabled(bool enabled)
	{
		m_jumpControlEnabled = enabled;
    }
}
