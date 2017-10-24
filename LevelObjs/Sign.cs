using UnityEngine;
using System.Collections;

public class Sign : MonoBehaviour
{
	private Transform[] m_children;

	void Start()
	{
		m_children = GetComponentsInChildren<Transform>();
		SetChildrenActive(false);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player)
		{
			SetChildrenActive(true);
        }
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.player)
		{
			SetChildrenActive(false);
		}
	}

	void SetChildrenActive(bool active)
	{
		for (int i = 1; i < m_children.Length; i++)
		{
			if (m_children[i].gameObject.name == "Tischplatte" ||
				m_children[i].gameObject.name == "Pfosten")
				continue;

			m_children[i].gameObject.SetActive(active);
        }
    }
}
