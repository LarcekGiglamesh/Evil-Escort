using UnityEngine;
using System.Collections;

public enum SwitchDetectionSide
{
	BOTH,
    LEFT,
	RIGHT
}

public class Switch : MonoBehaviour
{
	public GameObject[] m_linkedObjs;
	public bool m_destroyMeAfterActivation = false;
	public SwitchDetectionSide m_switchDetectionSide;

	private FallingTreeTrunk m_fallingTreeTrunk;


	void Start()
	{
		m_fallingTreeTrunk = GetComponent<FallingTreeTrunk>();
	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.basicAttack)
		{

			BasicAttack basicAttack = other.GetComponent<BasicAttack>();
			GameObject playerObj = basicAttack.GetOwner();
			float facingDir = playerObj.GetComponent<PlayerMovement>().GetFacingDir();
			switch (m_switchDetectionSide)
			{
				case SwitchDetectionSide.LEFT:
					if(facingDir != 1.0f)
						return;
					break;
				case SwitchDetectionSide.RIGHT:
					if (facingDir != -1.0f)
						return;
					break;
			}

			if (m_fallingTreeTrunk != null)
			{
				m_fallingTreeTrunk.StartAnimation(m_destroyMeAfterActivation, facingDir);
				GetComponent<BoxCollider2D>().enabled = false;
			}

			for (int i = 0; i < m_linkedObjs.Length; i++)
			{
				if (m_linkedObjs[i].tag == StringManager.Tags.door)
				{
					Door door = m_linkedObjs[i].GetComponent<Door>();
					door.SwitchState();
				}
				else
				{
					m_linkedObjs[i].SetActive(!m_linkedObjs[i].activeInHierarchy);
				}
			}

			if (m_fallingTreeTrunk == null && m_destroyMeAfterActivation)
			{
				Destroy(this.gameObject);
			}
        }
	}
}
