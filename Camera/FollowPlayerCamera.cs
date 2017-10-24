using UnityEngine;
using System.Collections;


public class FollowPlayerCamera : MonoBehaviour
{
	[Range(0.1f, 5.0f)]
	public float m_lerpFactorCameraLimit = 1.0f;

	private Transform m_cameraTargetPlayer;
	private Transform m_playerTrans;
	private Transform m_npcTrans;

	public Vector3 m_offset;
	public float m_lagFactor = 1.0f;
	public float m_zoomOutSpeed = 5.0f;
	public float m_zoomInSpeed = 5.0f;
	public float m_maxZoomOut = -40.0f;
	public float m_maxZoomIn = -15.0f;
	public int m_pixelOffsetNPC = 64;

	public bool m_ignoreRRH = false;

	private Transform m_cameraTargetCurrent;
	private bool m_zoomEnabled = true;

	private bool m_wasNpcOnScreen = true;


	public void Init()
	{
		// Find Player/NPC Transforms and CameraTarget Transforms
		GameObject playerObj = GameObject.FindGameObjectWithTag(StringManager.Tags.player);
		GameObject npcObj = GameObject.FindGameObjectWithTag(StringManager.Tags.redRidingHood);
		if(playerObj != null)
		{
			m_playerTrans = playerObj.transform;

			m_cameraTargetPlayer = m_playerTrans.FindChild(StringManager.Names.cameraTargetPlayer);
		}
		if(npcObj != null)
		{
			m_npcTrans = npcObj.transform;
        }
		// Set Current Camera Target
		m_cameraTargetCurrent = m_cameraTargetPlayer;

		// Do not show Arrow in Level without RRH
		if (m_ignoreRRH)
			m_wasNpcOnScreen = true;
    }

	void LateUpdate()
	{
		if (m_cameraTargetCurrent == null ||
			(m_npcTrans == null && !m_ignoreRRH))
			return;

		if(m_zoomEnabled && !m_ignoreRRH)
		{
			if (!IsNPCOnScreen())
			{
				m_offset.z -= m_zoomOutSpeed * Time.deltaTime;
				if (m_offset.z < m_maxZoomOut)
				{
					m_offset.z = m_maxZoomOut;
				}
			}
			if (IsNPCOnScreen())
			{
				m_offset.z += m_zoomInSpeed * Time.deltaTime;
				if (m_offset.z > m_maxZoomIn)
				{
					m_offset.z = m_maxZoomIn;
				}
			}
		}

		// Move Camera
		transform.position = GetLerpPos(transform.position);
		transform.position = GetLimitedPos(transform.position);
	}

	private bool IsNPCOnScreen()
	{
		bool ret = true;

		Vector3 prevCamPos = transform.position;
		transform.position = m_cameraTargetCurrent.position + m_offset;

		// Get ScreenPos of NPC
		Vector3 screenPosNPC = Camera.main.WorldToScreenPoint(m_npcTrans.position);
		
		// If NPC is not on Screen
		if (screenPosNPC.x < m_pixelOffsetNPC ||
			screenPosNPC.y < m_pixelOffsetNPC ||
			screenPosNPC.x > Camera.main.pixelWidth - m_pixelOffsetNPC ||
			screenPosNPC.y > Camera.main.pixelHeight - m_pixelOffsetNPC)
		{
			ret = false;
        }
		transform.position = prevCamPos;

		m_wasNpcOnScreen = ret;

		return ret;
    }

	public void SetTarget(Transform newTarget)
	{
		m_cameraTargetCurrent = newTarget;
	}

	public void SetZoomEnabled(bool enabled)
	{
		m_zoomEnabled = enabled;
	}

	private Vector3 GetLimitedPos(Vector3 pos)
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag(StringManager.Tags.player);

		GameObject[] limitRects = GameObject.FindGameObjectsWithTag(StringManager.Tags.cameraLimitRect);
		float shortestDistance = Mathf.Infinity;
		int indexShortestDistance = -1;
		for (int i = 0; i < limitRects.Length; i++)
		{
			float sqrDistance = (limitRects[i].transform.position - playerObj.transform.position).sqrMagnitude;
			if (sqrDistance < shortestDistance)
			{
				shortestDistance = sqrDistance;
				indexShortestDistance = i;
            }
        }





		GameObject limitRectObj = limitRects[indexShortestDistance];
		Vector2 limitPos = (Vector2)limitRectObj.transform.position;
		Vector2 limitScale = (Vector2)limitRectObj.transform.localScale * 10.0f;

		float limitLeft = limitPos.x - limitScale.x / 2.0f;
		float limitRight = limitPos.x + limitScale.x / 2.0f;

		float limitTop = limitPos.y + limitScale.y / 2.0f;
		float limitBottom = limitPos.y - limitScale.y / 2.0f;

		//Debug.Log("limitLeft = " + limitLeft + ", limitRight = " + limitRight + ", limitTop = " + limitTop + ", limitBottom = " + limitBottom);

		

		if (pos.x < limitLeft)
			pos.x = Mathf.Lerp(pos.x, limitLeft, m_lerpFactorCameraLimit * Time.deltaTime);
		if (pos.x > limitRight)
			pos.x = Mathf.Lerp(pos.x, limitRight, m_lerpFactorCameraLimit * Time.deltaTime);
		if (pos.y > limitTop)
			pos.y = Mathf.Lerp(pos.y, limitTop, m_lerpFactorCameraLimit * Time.deltaTime);
		if (pos.y < limitBottom)
			pos.y = Mathf.Lerp(pos.y, limitBottom, m_lerpFactorCameraLimit * Time.deltaTime);





		return pos;
	}

	private Vector3 GetLerpPos(Vector3 pos)
	{
		return Vector3.Lerp(pos, m_cameraTargetCurrent.position + m_offset, m_lagFactor * Time.deltaTime);
	}

	public Vector3 GetInitPos()
	{
		Vector3 initPos = m_cameraTargetCurrent.position + m_offset;
		initPos = GetLimitedPos(initPos);
		return initPos;
	}

	public bool WasNPCOnScreen()
	{
		return m_wasNpcOnScreen;
	}

	public Vector2 GetRRHScreenPos()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(m_npcTrans.position);
        return screenPos;
	}
}
