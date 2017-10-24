using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{
	float maxClimbAngle = 45;
	float maxDescendAngle = 80;
	float m_climbSpeedModifier = 0.5f;
	float m_descendSpeedModifier = 0.5f;

	public CollisionInfo collisions;
	public bool jumpEnabled = true;


	public override void Start()
	{
		base.Start();

	}

	public void Move(Vector3 velocity, bool standingOnPlatform = false)
	{
		UpdateRaycastOrigins();
		collisions.Reset();
		collisions.velocityOld = velocity;

		if (velocity.y < 0)
		{
			DescendSlope(ref velocity);
		}
		if (velocity.x != 0)
		{
			HorizontalCollisions(ref velocity);
		}
		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}

		transform.Translate(velocity);

		if (standingOnPlatform)
		{
			collisions.below = true;
		}
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			for (int j = 0; j < hits.Length; j++)
			{
				RaycastHit2D hit = hits[j];

				if (hit)
				{
					if (hit.transform.tag == StringManager.Tags.pickupObj)
					{
						if(hit.transform != this.transform)
						{
							PickupObj pickupObj = hit.transform.GetComponent<PickupObj>();

							float otherDirX = 0.0f;
							float otherVelocityX = pickupObj.GetMoveDir().x;
							if (otherVelocityX < 0.0f)
								otherDirX = -1.0f;
							else if (otherVelocityX > 0.0f)
								otherDirX = 1.0f;

							if (otherDirX == 0.0f || directionX == otherDirX)
							{
								pickupObj.SetMoveDir(new Vector2(velocity.x, 0.0f));
							}
						}
					}
					if (hit.transform == this.transform)
					{
						continue;
					}
					if (hit.distance == 0)
					{
						continue;
					}
					if (hit.transform.tag == StringManager.Tags.platformJumpThrough)
					{
						continue;
					}

					float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
					// adjust speed
					if (slopeAngle > 0.0f && slopeAngle <= maxClimbAngle)
					{
						velocity = velocity / (1.0f + slopeAngle * Mathf.Deg2Rad * m_climbSpeedModifier);
					}

					if (i == 0 && slopeAngle <= maxClimbAngle)
					{
						if (collisions.descendingSlope)
						{
							collisions.descendingSlope = false;
							velocity = collisions.velocityOld;
						}
						float distanceToSlopeStart = 0;
						if (slopeAngle != collisions.slopeAngleOld)
						{
							distanceToSlopeStart = hit.distance - skinWidth;
							velocity.x -= distanceToSlopeStart * directionX;
						}
						ClimbSlope(ref velocity, slopeAngle);
						velocity.x += distanceToSlopeStart * directionX;
					}

					if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
					{
						velocity.x = (hit.distance - skinWidth) * directionX;
						rayLength = hit.distance;

						if (collisions.climbingSlope)
						{
							velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
						}

						if (hit.transform.tag != StringManager.Tags.jumpPlatform)
						{
							collisions.left = directionX == -1;
							collisions.right = directionX == 1;
						}
					}

					
				}
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			for (int j = 0; j < hits.Length; j++)
			{
				RaycastHit2D hit = hits[j];
				if (hit)
				{
					if (hit.transform.tag == StringManager.Tags.pickupObj && this.tag == StringManager.Tags.pickupObj)
					{
						if (hit.transform == this.transform)
						{
							continue;
						}
					}
					if (hit.transform == this.transform)
					{
						continue;
					}
					if (hit.transform.tag == StringManager.Tags.platformJumpThrough && directionY == 1)
					{
						continue;
					}

					velocity.y = (hit.distance - skinWidth) * directionY;
					rayLength = hit.distance;

					if (collisions.climbingSlope)
					{
						velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
					}

					if (hit.transform.tag != StringManager.Tags.jumpPlatform)
					{
						collisions.below = directionY == -1;
						collisions.above = directionY == 1;
					}

					
				}
			}

			
		}

		// Jump Enable
		float dirX = Mathf.Sign(velocity.x);
		Vector2 ray = ( dirX == 1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight );  
		RaycastHit2D hitJump = Physics2D.Raycast(ray, Vector2.down, Mathf.Infinity, collisionMask);
		if (hitJump && (velocity.x >= 0.01 || velocity.x <= -0.01))
		{
			float slopeAngle = Vector2.Angle(hitJump.normal, Vector2.up);
			if (slopeAngle > maxClimbAngle)
			{
				jumpEnabled = false;
			}
			else
			{
				jumpEnabled = true;
			}
		}


		if (collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			for (int j = 0; j < hits.Length; j++)
			{
				RaycastHit2D hit = hits[j];

				if (hit)
				{
					if (hit.transform.tag == StringManager.Tags.pickupObj)
					{
						if (this.tag == StringManager.Tags.pickupObj)
						{
							continue;
						}
					}
					float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
					if (slopeAngle != collisions.slopeAngle)
					{
						velocity.x = (hit.distance - skinWidth) * directionX;
						collisions.slopeAngle = slopeAngle;
					}
				}
			}
			
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY)
		{
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if (Mathf.Sign(hit.normal.x) == directionX)
				{
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{
						// adjust speed
						velocity = velocity * (1.0f + slopeAngle * Mathf.Deg2Rad * m_descendSpeedModifier);

						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}



	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}