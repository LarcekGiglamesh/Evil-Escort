using UnityEngine;
using System.Collections;

public class BookmarkLerpSingle : MonoBehaviour {

	// Bookmark Lerp
	public GameObject m_BookmarkObject = null;
	public Color m_BookmarkLerpBegin;
	public Color m_BookmarkLerpEnd;
	private float m_FadingValue = 0.0f;
	private float m_FadingKey = 1.0f;
	
	// Update is called once per frame
	void Update () {

		m_FadingValue += (Time.deltaTime * m_FadingKey);
		if (m_FadingValue > 1.0f) m_FadingKey = -1.0f;
		if (m_FadingValue < 0.0f) m_FadingKey = 1.0f;
		m_BookmarkObject.GetComponent<Renderer>().material.color = Vector4.Lerp(m_BookmarkLerpBegin, m_BookmarkLerpEnd, m_FadingValue);

	}
}
