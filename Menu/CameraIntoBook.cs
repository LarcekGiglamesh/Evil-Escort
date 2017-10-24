using UnityEngine;
using System.Collections;

public class CameraIntoBook : MonoBehaviour
{
	private Vector3 m_cameraStartPos;
	private Quaternion m_cameraStartRotation;

	public Transform m_endTrans; // Get End-Position and Rotation from this Transform

	private float m_timer = Mathf.Infinity;
	private int m_levelIndex = -1;

	void Start()
	{
		m_cameraStartPos = Camera.main.transform.position;
		m_cameraStartRotation = Camera.main.transform.localRotation;
    }

	void Update()
	{
		if (m_timer < 1.0f)
		{
			Camera.main.transform.position = Vector3.Lerp(m_cameraStartPos, m_endTrans.position, m_timer);
			Camera.main.transform.rotation = Quaternion.Slerp(m_cameraStartRotation, m_endTrans.localRotation, m_timer);

			m_timer += Time.deltaTime;

			if (m_timer >= 1.0f)
			{
				OnTransitionEnd();
            }
        }
	}

	public void StartCameraTransition(int levelIndex)
	{
		m_timer = 0.0f;
		m_levelIndex = levelIndex;
    }

	private void OnTransitionEnd()
	{
		Singleton.AudioManager.StopBGM();
		Singleton.AudioManager.PlayBGM(BGMName.ingame, 0.0f);
		Application.LoadLevel(m_levelIndex + GameConstants.LEVEL_ID_OFFSET);
	}
}
