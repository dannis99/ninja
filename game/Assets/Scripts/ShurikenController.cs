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
				collision.gameObject.GetComponent<PlayerController>().takeDamage(player);
				Destroy(gameObject);
			}
			else
			{
                base.collision(collision.gameObject);
			}
		}
	}
}
