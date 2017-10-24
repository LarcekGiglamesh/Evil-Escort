using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DragObjState
{
	NONE,
	DRAG,
	DROP,
}

public class PlayerAttack : MonoBehaviour
{
	public int m_damage = 1;
	public float m_cooldownTime = 1.0f;

	public Transform m_attackLeftPos;
	public Transform m_attackRightPos;
	public Transform m_holdLeftPos;
	public Transform m_holdRightPos;

	private Animator m_animator;
	
	private PlayerMovement m_playerMovement;
	private float m_timerCooldown;

	private DragObjState m_dragState = DragObjState.NONE;
	private List<Transform> m_dragObjs = new List<Transform>();
	private float m_dragDir = 0.0f;

	private bool m_prevAttackInput = false;

	void Start()
	{
		m_playerMovement = GetComponent<PlayerMovement>();
		m_animator = GetComponentInChildren<Animator>();
    }

	void Update()
	{
		if (m_timerCooldown > 0.0f)
		{
			m_timerCooldown -= Time.deltaTime;
        }

		// Get Input
		XInput.XInputState state;
		XInput.XInputGetState(0, out state);
		bool attackInputDown = false;
		if (Singleton.GameVarsManager.m_gameVars.m_keyboardControl)
		{
			attackInputDown = Input.GetKeyDown(KeyCode.Return);
		}
		else
		{
			bool currentAttackInput = state.Gamepad.IsButtonDown(XInput.XInputButtons.X);
			if (!m_prevAttackInput && currentAttackInput)
			{
				attackInputDown = true;
			}
			m_prevAttackInput = currentAttackInput;
		}
		bool attackInputUp = false;
		if (Singleton.GameVarsManager.m_gameVars.m_keyboardControl)
		{
			attackInputUp = Input.GetKeyUp(KeyCode.Return);
		}
		else
		{
			attackInputUp = state.Gamepad.IsButtonDown(XInput.XInputButtons.X) == false;
		}


		// Attack
		if (attackInputDown)
		{
			if (m_timerCooldown <= 0.0f)
			{
				m_dragState = DragObjState.DRAG;
				m_dragDir = -1.0f * m_playerMovement.GetFacingDir();

				Vector3 pos = Vector3.zero;
				if (m_playerMovement.GetFacingDir() == -1.0f)
				{
					pos = m_attackLeftPos.position;
				}
				else
				{
					pos = m_attackRightPos.position;
				}
				// Attack reaches target after 5/24 seconds
				GameObject basicAttackObj = (GameObject) GameObject.Instantiate(Resources.Load(StringManager.Resources.basicAttack),
					pos, Quaternion.identity);
				basicAttackObj.GetComponent<BasicAttack>().m_damage = m_damage;
				basicAttackObj.GetComponent<BasicAttack>().SetOwner(this.gameObject);

				m_timerCooldown = m_cooldownTime;
			}
		}
		
		if (attackInputUp)
		{
			m_dragState = DragObjState.DROP;
			m_dragDir = 0.0f;
        }

		if(m_dragState == DragObjState.DROP)
		{
			DropObjs();
        }

		if(m_dragState == DragObjState.DRAG)
		{
			if (!m_playerMovement.IsOnGround())
			{
				DropObjs();
			}
		}


		
	}

	public void DropObjs()
	{
		Transform[] children = transform.GetComponentsInChildren<Transform>();
		for (int i = 1; i < children.Length; i++)
		{
			if (children[i].tag == StringManager.Tags.pickupObj)
			{
				children[i].SetParent(null);
				children[i].GetComponent<PickupObj>().enabled = true;

				//children[i].GetComponent<Collider2D>().enabled = true;
				//children[i].GetComponent<Controller2D>().enabled = true;
			}
		}

		m_dragObjs.Clear();
    }

	public void AddPickupObj(Transform objTrans)
	{
		m_dragObjs.Add(objTrans);
    }

	public int GetCountPickupObjs()
	{
		return m_dragObjs.Count;
	}

	public bool IsDraggingObjs()
	{
		return m_dragState == DragObjState.DRAG;
	}

	public float GetDragDir()
	{
		return m_dragDir;
	}

	public void OnAttackStart()
	{
		m_animator.SetBool("isAttacking", true);
	}

	public void OnAttackEnd()
	{
		m_animator.SetBool("isAttacking", false);
	}
}
