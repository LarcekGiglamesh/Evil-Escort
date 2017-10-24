using UnityEngine;
using System.Collections;

public class BackgroundPlane : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		float z = pos.z;
		pos = Camera.main.transform.position;
		pos.z = z;
		transform.position = pos;
	}
}
