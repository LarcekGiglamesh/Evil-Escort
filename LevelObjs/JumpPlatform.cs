using System;
using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour
{
	public Vector2 m_jumpForce = Vector2.zero;
	private Animator m_Animator;

	void Start()
	{
		m_Animator = GetComponent<Animator>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player ||
			other.tag == StringManager.Tags.redRidingHood ||
			other.tag == StringManager.Tags.pickupObj)
		{
			BaseMover mover = other.gameObject.GetComponent<BaseMover>();
			mover.SetVelocity(Vector2.zero);
			mover.SetForce(m_jumpForce);
			mover.SetJumpControlEnabled(false);

			Controller2D ctrl2D = other.GetComponent<Controller2D>();
			ctrl2D.collisions.below = false;
			ctrl2D.collisions.above = false;

			m_Animator.SetTrigger("doAnim");
			
			if (other.tag == StringManager.Tags.player)
			{
				other.GetComponent<PlayerMovement>().SetIsJumping(true);
            }

			if (other.tag == StringManager.Tags.redRidingHood)
			{
				other.GetComponent<RedRidingHood>().RrhDoJump();
			}
		}
	}

}
