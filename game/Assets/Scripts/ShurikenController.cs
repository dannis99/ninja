using UnityEngine;
using System.Collections;

public class ShurikenController : MonoBehaviour {

	new Rigidbody2D rigidbody2D;
	Collider2D[] colliders;
	bool active = true;
	Vector2 velocity = Vector2.zero;

	// Use this for initialization
	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
		colliders = GetComponents<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = velocity;
	}

	public void setVelocity(Vector2 newVelocity)
	{
		velocity = newVelocity;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(active)
		{
			active = false;
			if(collision.gameObject.tag == "player")
			{
				collision.gameObject.GetComponent<PlayerController>().takeDamage();
				Destroy(gameObject);
			}
			else
			{
				velocity = Vector2.zero;
				rigidbody2D.isKinematic = true;

				foreach(Collider2D collider in colliders)
				{
					collider.enabled = false;
				}
			}
		}
	}
}
