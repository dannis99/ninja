using UnityEngine;
using System.Collections;

public class ShurikenController : ShurikenParentController {

	bool active = true;
	
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
				shurikenRigidbody2D.isKinematic = true;

				foreach(Collider2D collider in colliders)
				{
					collider.enabled = false;
				}
			}
		}
	}
}
