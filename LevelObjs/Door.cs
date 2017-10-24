using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
	public Transform[] m_doorObjs;

	private bool m_isOpen = false;
	
	public void SwitchState()
	{
		if (m_isOpen)
			Close();
		else
			Open();
	}

	public virtual void Open()
	{
		if (m_isOpen)
			return;

		m_isOpen = true;
	}

	public virtual void Close()
	{
		if (!m_isOpen)
			return;

		m_isOpen = false;
	}
	
}
