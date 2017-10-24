using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArrowRRH : MonoBehaviour
{
	// private RawImage m_arrowImage;
	// private FollowPlayerCamera m_fpCam;
	// private RectTransform m_rectTrans;
	// private Transform m_rrhTrans;

	void Start()
	{
		// m_fpCam = Camera.main.GetComponent<FollowPlayerCamera>();
		// m_arrowImage = GetComponent<RawImage>();
		// m_rectTrans = GetComponent<RectTransform>();
		// m_rrhTrans = GameObject.FindGameObjectWithTag(StringManager.Tags.redRidingHood).transform;
    }

	void LateUpdate()
	{
		//if (m_fpCam.WasNPCOnScreen())
		//{
		//	m_arrowImage.enabled = false;
		//}
		//else
		//{
		//	m_arrowImage.enabled = true;
		//}

		//Vector2 rrhScreenPos = m_fpCam.GetRRHScreenPos();
		//Debug.Log(rrhScreenPos);
		//m_rectTrans.anchoredPosition = rrhScreenPos;

		//Vector3 srcPos = Camera.main.transform.position;
		//srcPos.z = 0.0f;
		//Vector2 toRRH = (m_rrhTrans.position - srcPos).normalized;

		//Quaternion rot = Quaternion.Euler(toRRH)

		//m_arrowImage.rectTransform.rotation = rot;
    }
}
