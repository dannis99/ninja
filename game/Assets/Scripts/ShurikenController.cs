using UnityEngine;
using System.Collections;

public class ShurikenController : MonoBehaviour {

	new Rigidbody2D rigidbody2D;
	Collider2D[] colliders;
	bool active = true;

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		colliders = GetComponents<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(active)
		{
			active = false;
			if(collision.gameObject.tag == "player")
			{
				collision.gameObject.GetComponent<PlayerController>().takeDamage();
				Invoke("disappear", .5f);
			}
			else
			{
				rigidbody2D.isKinematic = true;

				foreach(Collider2D collider in colliders)
				{
					if(!collider.isTrigger)
					{
						collider.enabled = false;
					}
				}
			}
		}
	}

	void disappear()
	{
		Destroy(gameObject);
	}
}
