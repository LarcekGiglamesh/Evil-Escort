using UnityEngine;
using System.Collections;

public enum RopeObjLinkType
{
	NONE,
	FallingObstacle,
}

public class Rope : MonoBehaviour
{
	public GameObject m_linkedObj;
	public RopeObjLinkType m_ropeObjLinkType;

	void Start()
	{

	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == StringManager.Tags.basicAttack)
		{
			AffectLinkedObj();
			Destroy(this.gameObject);
		}
	}

	public void AffectLinkedObj()
	{
		switch(m_ropeObjLinkType)
		{
			case RopeObjLinkType.FallingObstacle:
				m_linkedObj.GetComponent<FallingObstacle>().SetIsFalling(true);
				break;
		}
	}
}
