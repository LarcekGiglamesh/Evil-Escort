using UnityEngine;
using System.Collections;

public class BasicAttack : MonoBehaviour
{
	public float m_lifeTime = 1.0f;
	public int m_damage = 1;

	private GameObject m_owner;
	private PlayerAttack m_playerAttackRef;
	private float m_timerLife;

	void Start()
	{
		m_playerAttackRef = m_owner.GetComponent<PlayerAttack>();
		m_playerAttackRef.OnAttackStart();
		m_timerLife = m_lifeTime;
	}

	void Update()
	{
		if (m_timerLife > 0.0f)
		{
			m_timerLife -= Time.deltaTime;

			if (m_timerLife <= 0.0f)
			{
				m_playerAttackRef.OnAttackEnd();
				Destroy(this.gameObject);
			}
		}

    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == StringManager.Tags.pickupObj)
		{
			PickUpObj(other.transform);
        }
	}

	private void PickUpObj(Transform objTrans)
	{
		objTrans.transform.SetParent(m_owner.transform);
		objTrans.GetComponent<PickupObj>().enabled = false;
		objTrans.GetComponent<PickupObj>().SetMoveDir(Vector2.zero);
		objTrans.GetComponent<PickupObj>().SetForce(Vector2.zero);

		//objTrans.GetComponent<Collider2D>().enabled = false;
		//objTrans.GetComponent<Controller2D>().enabled = false;

		PlayerAttack playerAttack = m_owner.GetComponent<PlayerAttack>();
		playerAttack.AddPickupObj(objTrans);
	}

	public void SetOwner(GameObject obj)
	{
		m_owner = obj;
	}

	public GameObject GetOwner()
	{
		return m_owner;
	}
}
