using UnityEngine;
using System.Collections;

public class TurnAround : MonoBehaviour
{
	
	void Start()
	{

	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == StringManager.Tags.redRidingHood)
		{
			RedRidingHood rrh = other.GetComponent<RedRidingHood>();
			rrh.TurnAround();
		}
	}
}
