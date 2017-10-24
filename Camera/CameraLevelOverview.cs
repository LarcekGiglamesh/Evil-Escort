using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraComparer : IComparer<Transform>
{
	public int Compare(Transform t1, Transform t2)
	{
		return (t2.position.x.CompareTo(t1.position.x));
	}
}

public class CameraLevelOverview : MonoBehaviour
{
	public bool m_debugDisable = false;
	[Range(1.0f, 10.0f)]
	public float m_timeIntervalBetweenPoints = 5.0f;
	public float m_zPos = -35.0f;

	private Transform[] m_levelOverviewPoints;
	private float m_timer = 0.0f;
	private int m_index = 0;
	private int m_prevIndex = 0;

	private FollowPlayerCamera m_followScript;
	private Vector3 m_lastPointPos;
	private Vector3 m_startFollowPos;
	private bool m_isLerpingToStartFollowPos = false;

	private RedRidingHood m_redRidingHood;
	private PlayerMovement m_playerMovement;
	private PlayerAttack m_playerAttack;

	void Start()
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag(StringManager.Tags.player);
		m_playerMovement = playerObj.GetComponent<PlayerMovement>();
		m_followScript = GetComponent<FollowPlayerCamera>();
		if (m_debugDisable)
		{
			m_playerMovement.m_movementEnabled = true;
			m_followScript.Init();
			return;
		}

		GameObject redRidingHoodObj = GameObject.FindGameObjectWithTag(StringManager.Tags.redRidingHood);
		m_redRidingHood = redRidingHoodObj.GetComponent<RedRidingHood>();
		m_redRidingHood.enabled = false;

		m_playerMovement.m_movementEnabled = false;
		m_playerAttack = playerObj.GetComponent<PlayerAttack>();
		m_playerAttack.enabled = false;

		// Disable Camera Follow Script
		m_followScript.enabled = false;
		// Find Objs
		GameObject[] objs = GameObject.FindGameObjectsWithTag(StringManager.Tags.cameraLevelOverviewPoint);
		// Get Transform Array
		m_levelOverviewPoints = new Transform[objs.Length];
		for (int i = 0; i < objs.Length; i++)
		{
			m_levelOverviewPoints[i] = objs[i].transform;
		}
		// Sort by Pos.x
		CameraComparer comparer = new CameraComparer();
		Array.Sort<Transform>(m_levelOverviewPoints, comparer);
		// Debug
		//foreach (Transform t in m_levelOverviewPoints)
		//{
		//	Debug.Log(t.position.x);
		//}

		m_prevIndex = 0;
		m_index = 1;

		m_followScript.Init();
	}

	void LateUpdate()
	{
		if (m_debugDisable)
		{
			return;
		}

		m_timer += Time.deltaTime;

		Vector3 offset = new Vector3(0, 0, m_zPos);
		if (!m_isLerpingToStartFollowPos)
		{
			// Move Camera
			transform.position = Vector3.Lerp(m_levelOverviewPoints[m_prevIndex].position + offset,
				m_levelOverviewPoints[m_index].position + offset,
				m_timer / m_timeIntervalBetweenPoints);
			
			if (m_timer > m_timeIntervalBetweenPoints)
			{
				m_prevIndex = m_index;
				m_index++;
				if (m_index > m_levelOverviewPoints.Length - 1)
				{
					m_index = m_levelOverviewPoints.Length - 1;

					// Lerp to start follow pos
					m_isLerpingToStartFollowPos = true;
					m_lastPointPos = m_levelOverviewPoints[m_levelOverviewPoints.Length - 1].position + offset;
					m_startFollowPos = m_followScript.GetInitPos();
					//Debug.Log("Last Point Pos =" + m_lastPointPos);
					//Debug.Log("Start Follow Pos =" + m_startFollowPos);

				}
				m_timer = 0.0f;
			}
		}
		else
		{
			// Move Camera
			transform.position = Vector3.Lerp(m_lastPointPos,
				m_startFollowPos,
				m_timer / m_timeIntervalBetweenPoints);

			if (m_timer > m_timeIntervalBetweenPoints)
			{
				// Start CameraFollow Script and disable this Script
				m_followScript.enabled = true;
				this.enabled = false;

				// If not in Tutorial
				if (Application.loadedLevel != 0)
				{
					m_redRidingHood.enabled = true;
				}

				m_playerMovement.m_movementEnabled = true;
				m_playerAttack.enabled = true;
			}
        }
	}
}
