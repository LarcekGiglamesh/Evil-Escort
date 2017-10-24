using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
    public float roationSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
		// this.transform.rotation *= Quaternion.Euler(new Vector3(0, roationSpeed * Time.deltaTime, 0));
		transform.Rotate(new Vector3(0, roationSpeed * Time.deltaTime, 0), Space.World);
	}
}
