using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour
{
	private static DontDestroyOnLoad m_instance;

	void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			DontDestroyOnLoad(this.transform);
		}
		else
		{
			DestroyImmediate(this.gameObject);
		}
	}
	
	void Update()
	{
	
	}
}
