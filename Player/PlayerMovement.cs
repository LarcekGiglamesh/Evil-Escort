using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : BaseMover
{
	public float m_jumpModifier = 0.5f;

	public Transform m_wolf;
	private Vector3 m_wolfScale;

	private float m_velocityXSmoothing;
	private float m_facingDir = 1.0f;
	private float m_facingThreshold = 0.1f;

	private PlayerAttack m_playerAttack;
	private Animator m_animator;

	private bool m_isJumpingAnim = false;

	private float m_timerBeforeJump = 0.0f;

	private bool m_prevJumpInput = false;

	public bool m_movementEnabled = false;

	private float m_timeStart = 0.0f;

	private AudioSource m_walkAudioSource;
	private bool m_wasWalking; // in prev frame
	private int m_countWalking; // count frames after walk state change
	private bool m_countWalkingStarted;

	override protected void Start()
	{
		m_controller2D = GetComponent<Controller2D>();
		m_wolfScale = m_wolf.localScale;
		m_playerAttack = GetComponent<PlayerAttack>();
		m_animator = m_wolf.GetComponent<Animator>();

		m_timeStart = 0.0f;
    }

	override protected void Update()
	{
		base.Update();
		m_timeStart += Time.deltaTime;

		// Get Input
		Vector2 input = Vector2.zero;
		bool jumpInput = false;
		if (m_movementEnabled)
		{
			if (Singleton.GameVarsManager.m_gameVars.m_keyboardControl)
			{
				input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
				jumpInput = Input.GetKey(KeyCode.Space);
			}
			else
			{
				XInput.XInputState state;
				XInput.XInputGetState(0, out state);
				input = new Vector2(state.Gamepad.LeftThumbX / 32768.0f, state.Gamepad.LeftThumbY / 32768.0f);
				input.Normalize();
				jumpInput = state.Gamepad.IsButtonDown(XInput.XInputButtons.A);
            }
		}

		// Jump
		if (m_prevJumpInput == false && jumpInput
			&& m_controller2D.collisions.below
			&& m_controller2D.jumpEnabled
			&& m_timerBeforeJump <= 0.0f)
		{
			m_isJumpingAnim = true;

			m_velocity.y += m_jumpVelocity;
			m_jumpControlEnabled = true;
			m_playerAttack.DropObjs();
		}

		// Debug.Log("Wolf VSpeed: " + ((float)m_velocity.y * input.y) + " | velocity: " + m_velocity.y + " | facing: " + input.y);


		if (m_jumpControlEnabled)
		{
			if (Input.GetKeyUp(KeyCode.Space) && !m_controller2D.collisions.below)
			{
				if (m_velocity.y > 0.0f)
				{
					m_velocity.y -= m_jumpVelocity * (m_velocity.y / m_jumpVelocity) * m_jumpModifier;
				}
			}
		}

		// Smooth X Movement
		float targetVelocityX = input.x * m_speed;
		m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVelocityX, ref m_velocityXSmoothing,
			(m_controller2D.collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);

		// Gravity
		m_velocity.y += m_gravity * Time.deltaTime;
		
		// Move by using Raycast Controller 2D
		m_controller2D.Move((m_force + m_velocity) * Time.deltaTime);

		// Update FacingDir
		if (m_velocity.x > m_facingThreshold)
		{
			m_facingDir = 1.0f;
			m_wolf.transform.localScale = new Vector3(m_wolfScale.x, m_wolfScale.y, m_wolfScale.z);
		}
		else if (m_velocity.x < -m_facingThreshold)
		{
			m_wolf.transform.localScale = new Vector3(m_wolfScale.x, m_wolfScale.y, -m_wolfScale.z);
			m_facingDir = -1.0f;
		}


		bool isWalking = (m_velocity.x <= -0.1f || m_velocity.x >= 0.1f);

		if (isWalking != m_wasWalking)
		{
			m_countWalkingStarted = true;
			m_countWalking = 0;
		}

		if (m_countWalkingStarted)
		{
			m_countWalking++;
		}

		if (!m_countWalkingStarted || (m_countWalkingStarted && m_countWalking > 2))
		{
			m_countWalkingStarted = false;

			if (isWalking)
			{
				m_animator.SetBool("isWalking", true);

				if (IsOnGround())
				{
					if (m_walkAudioSource == null)
					{
						m_walkAudioSource = Singleton.AudioManager.PlaySFX(SFXName.wolfSteps);
					}
				}
			}
			else
			{
				//Debug.Log("not walking");

				if (m_walkAudioSource != null)
				{
					Singleton.AudioManager.StopSFX(m_walkAudioSource);
				}

				m_animator.SetBool("isWalking", false);
			}
		}

		

		m_wasWalking = isWalking;

		if (IsOnGround() && m_timerBeforeJump <= 0.0f)
		{
			m_isJumpingAnim = false;
		}
		m_animator.SetBool("isJumping", m_isJumpingAnim);

		if (m_controller2D.collisions.climbingSlope)
		{
			m_playerAttack.DropObjs();
		}

		m_prevJumpInput = jumpInput;


		m_animator.SetBool("isOnGround", IsOnGround());
		m_animator.SetFloat("verticalSpeed", (m_force.y + m_velocity.y));
	}

	public float GetFacingDir()
	{
		return m_facingDir;
	}

	public override void OnDeath()
	{
		base.OnDeath();
		StatsTracker.OnLevelFailed();
		Destroy(this.gameObject);
	}

	public bool IsOnGround()
	{
		return m_controller2D.collisions.below;
	}

	public void SetIsJumping(bool isJumping)
	{
		m_isJumpingAnim = isJumping;
	}
}
