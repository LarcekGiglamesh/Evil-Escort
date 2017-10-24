using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RedRidingHood : BaseMover
{
	public Vector2 m_input;
	public float m_rageIncreasePerSec;
	public float m_rageDecreasePerSec;
	private const float m_rageBarThreshold = 0.15f;

	private float m_velocityXSmoothing;
	private Image m_rageBar;
	private float m_rageValue = 1.0f;
	private bool m_hasLost = false;

	private Slider m_slider;
	private float m_sliderMin = 0.0f;
	private float m_sliderMax = 0.0f;
	private float m_relativePos = 0.0f;

	public Animator m_animatorObject;

	private bool m_prevRageState = false;

	private Vector2 m_prevPos;
	public float m_deltaX;

	//private float m_timeJumpStarted;

	private int m_counter;

	override protected void Start()
	{
		base.Start();
		m_rageBar = GameObject.Find(StringManager.Names.rageBarFG).GetComponent<Image>();

		// Slider Data
		GameObject sliderObj = GameObject.Find(StringManager.Names.levelSlider);
		if(sliderObj != null)
			m_slider = sliderObj.GetComponent<Slider>();

		GameObject sliderMin = GameObject.Find(StringManager.Names.levelLeftBorder);
		if(sliderMin != null)
			m_sliderMin = sliderMin.transform.position.x;

		GameObject sliderMax = GameObject.Find(StringManager.Names.levelRightBorder);
		if(sliderMax != null)
			m_sliderMax = sliderMax.transform.position.x;

		m_rageBar.fillAmount = m_rageValue;

		m_prevPos = transform.position;
    }

	override protected void Update()
	{
		base.Update();



		float targetVelocityX = m_input.x * m_speed;

		m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVelocityX, ref m_velocityXSmoothing,
			(m_controller2D.collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);

		if((m_controller2D.collisions.right ||
			m_controller2D.collisions.left) &&
			m_controller2D.collisions.below)
		{
			//m_velocity.y = m_jumpVelocity;
			IncreaseRage();
        }
		else if(m_controller2D.collisions.above)
		{
			IncreaseRage();
		}
		else
		{
			DecreaseRage();
        }

		m_velocity.y += m_gravity * Time.deltaTime;
		m_controller2D.Move((m_force + m_velocity) * Time.deltaTime);

		if ( m_slider != null)
		{
			UpdateSliderPos();
		}

		m_rageBar.fillAmount = m_rageValue;

		bool currentRageState = false;
		if (m_rageValue < 1.0f - m_rageBarThreshold && !m_hasLost)
		{
			currentRageState = true;
		}
		if (currentRageState && m_prevRageState == false)
		{
			StatsTracker.m_numRageBarActivated++;
		}
		m_prevRageState = currentRageState;




		m_deltaX = Mathf.Abs(transform.position.x - m_prevPos.x);
		
		if (m_deltaX <= 0.0001f)
		{
			m_counter++;
        }
		else
		{
			m_animatorObject.SetBool("isWalking", true);
		}

		if (m_counter > 20)
		{
			m_animatorObject.SetBool("isWalking", false);
			m_counter = 0;
        }
		

		m_animatorObject.SetBool("isOnGround", m_controller2D.collisions.below);
		if (m_animatorObject.GetBool("isJumping"))
		{
			m_animatorObject.SetBool("isJumping", !m_controller2D.collisions.below);
		}


		m_prevPos = transform.position;

		
	}

	void IncreaseRage()
	{
		if (m_deltaX > 0.0001f)
		{
			return;
		}

		if (m_hasLost)
			return;

		m_rageValue -= m_rageIncreasePerSec * Time.deltaTime;
		if (m_rageValue <= 0.0f)
		{
			m_rageValue = 0.0f;
			m_hasLost = true;
			StatsTracker.OnLevelFailed();
		}

		if (m_hasLost)
		{
			TurnAround();
			m_speed *= 2.0f;

			FollowPlayerCamera cameraScript = Camera.main.GetComponent<FollowPlayerCamera>();
			Transform childCamTarget = transform.FindChild(StringManager.Names.cameraTargetRedRidingHood);
            cameraScript.SetTarget(childCamTarget);
			childCamTarget.SetParent(null);
			cameraScript.SetZoomEnabled(false);
		}
	}

	void DecreaseRage()
	{
		if (m_hasLost)
		{
			return;
		}

		m_rageValue += m_rageDecreasePerSec * Time.deltaTime;
		if (m_rageValue >= 1.0f)
		{
			m_rageValue = 1.0f;
		}
    }

	//TODO: void OnXSecondsAfterLevelLost()

	public override void OnDeath()
	{
		base.OnDeath();
		StatsTracker.OnLevelFailed();
		Destroy(this.gameObject);
	}

	void UpdateSliderPos()
	{
		float diff = -m_sliderMin;
		// Debug.Log("New Pos: " + (transform.position.x- diff));

		m_relativePos = (transform.position.x + diff) / (m_sliderMax + diff);
		m_slider.value = m_relativePos;
	}

	public void RrhDoJump()
	{
		m_animatorObject.SetBool("isJumping", true);
		//m_timeJumpStarted = Time.time;
    }

	public float GetRageValue()
	{
		return m_rageValue;
	}

	public void TurnAround()
	{
		m_input.x *= -1.0f;
		Vector3 scale = m_animatorObject.transform.localScale;
		scale.z *= -1.0f;
		m_animatorObject.transform.localScale = scale;
	}
}
