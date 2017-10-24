using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameVars
{
	public bool m_keyboardControl;

	public GameVars()
	{
		Reset();
	}

	public void Reset()
	{
		m_keyboardControl = true;
    }
}

public class GameVarsManager : MonoBehaviour
{
	public GameVars m_gameVars;

	void Start()
	{
		Reset();

		m_gameVars.m_keyboardControl = !XInput.IsControllerConnected(0);
	}

	void Update()
	{
		m_gameVars.m_keyboardControl = !XInput.IsControllerConnected(0);
	}

	public void Reset()
	{
		m_gameVars.Reset();
	}

}
