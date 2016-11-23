using UnityEngine;
using System.Collections;

public class PenetratingShurikenController : ShurikenParentController {

	bool active = true;
	
    public override void OnTriggerEnter2D(Collider2D collider)
	{
		if(active)
		{
			if(collider.gameObject.tag == "player")
			{
				collider.gameObject.GetComponent<PlayerController>().takeDamage(player);
			}
			else if(collider.gameObject.tag == "surface")
			{
                collision(collider.gameObject);
			}
		}
	}
}
