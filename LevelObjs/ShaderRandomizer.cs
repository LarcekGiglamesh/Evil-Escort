using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShaderRandomizer : MonoBehaviour
{
	[Tooltip("Aktiviert zufaelligen Offset basierend auf Position in X zuzueglich Offset")]
	public bool _AddRandomOffset = false;
	[Tooltip("Werte zwischen 0.0 und 6.28318 (2 Pi)")]
	public float _RandomOffset = 3.1415f;
	[Tooltip("Interpolation ausgehend von Ursprung")]
	public Vector4 _WindDirection = new Vector4(27.33f, 0.0f, 34.0f, 1.0f);
	[Tooltip("Wind Staerke")]
	public float _WindPower = 0.33f;
	[Tooltip("Wind Geschwindigkeit")]
	public float _WindSpeed = 26.8f;
	
	public bool m_enableVertexShader; // TODO: implement

	void Start ()
	{
		float randomValue = (transform.position.x + _RandomOffset) % (2 * Mathf.PI);
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.shader = Shader.Find("Custom/GrassShaderLit");

		_WindDirection.w = 1.0f;

		float offsetActive = (_AddRandomOffset == true ? 1.0f : 0.0f);
		renderer.material.SetFloat("_AdditionalWindOffset", offsetActive);
        renderer.material.SetFloat("_Random", randomValue);
		renderer.material.SetVector("_WindDirection", _WindDirection);
		renderer.material.SetFloat("_WindPower", _WindPower);
		renderer.material.SetFloat("_WindSpeed", _WindSpeed);
	}
	
}
