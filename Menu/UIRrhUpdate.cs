using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIRrhUpdate : MonoBehaviour
{

	private RedRidingHood m_rrhScript;
	public float m_RageSecondState = 0.66f;
	public float m_RageThirdState = 0.33f;
	public float m_RageValue = 0.0f;

	public Sprite m_RageState01;
	public Sprite m_RageState02;
	public Sprite m_RageState03;

	public float m_SpeedGrowth = 0.2f;
	public float m_MaxRotation = 17.0f;
	public float m_MinRotation = -17.0f;
	private float m_CurRotation = 0.0f;
    private float m_factor = 1.0f;

    private const float m_ShakeState01 = 1.00f;
    private const float m_ShakeState02 = 4.75f;
    private const float m_ShakeState03 = 8.50f;

	// Use this for initialization
	void Start () {
		GameObject rrhObj = GameObject.Find(StringManager.Tags.redRidingHood);
		if (rrhObj != null)
			m_rrhScript = rrhObj.GetComponent<RedRidingHood>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_rrhScript == null)
			return;

		m_RageValue = m_rrhScript.GetRageValue();

		if (m_RageValue > m_RageSecondState)
		{
			GetComponent<Image>().sprite = m_RageState01;
		    m_factor = m_ShakeState01;
		}
        else if (m_RageValue < m_RageSecondState && m_RageValue > m_RageThirdState)
		{
			GetComponent<Image>().sprite = m_RageState02;
            m_factor = m_ShakeState02;
        }
		else if (m_RageValue < m_RageThirdState)
		{
			GetComponent<Image>().sprite = m_RageState03;
		    m_factor = m_ShakeState03;
		}

		// Rotation
		if(m_RageValue < m_RageSecondState)
		{
			m_CurRotation += ( m_SpeedGrowth * m_factor );
			if( m_CurRotation < m_MinRotation || m_CurRotation > m_MaxRotation)
			{
				m_SpeedGrowth *= -1;
			}
			Quaternion newRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_CurRotation));
            // Debug.Log("New Rotation: \t" + newRotation);
			GetComponent<RectTransform>().rotation = newRotation;
		}
	}
}
