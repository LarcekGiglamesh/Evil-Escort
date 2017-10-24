using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour
{
	public GameObject[] m_linkedObjs;
	public bool m_autoDeactivate = true;

	private bool m_isActivated = false;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(!m_isActivated)
		{
			SwitchState(other);
			m_isActivated = true;
        }
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (m_autoDeactivate && m_isActivated)
		{
			SwitchState(other);
			m_isActivated = false;
        }
	}

	void SwitchState(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player ||
			other.tag == StringManager.Tags.redRidingHood ||
			other.tag == StringManager.Tags.obstacle ||
			other.tag == StringManager.Tags.fallingObstacle ||
			other.tag == StringManager.Tags.pickupObj)
		{
			for (int i = 0; i < m_linkedObjs.Length; i++)
			{
				if(m_linkedObjs[i].tag == StringManager.Tags.door)
				{
					Door door = m_linkedObjs[i].GetComponent<Door>();
					door.SwitchState();
				}
				else
				{
					m_linkedObjs[i].SetActive(!m_linkedObjs[i].activeInHierarchy);
				}
			}
		}
	}

}
