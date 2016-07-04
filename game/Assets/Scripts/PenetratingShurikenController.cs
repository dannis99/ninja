﻿using UnityEngine;
using System.Collections;

public class PenetratingShurikenController : ShurikenParentController {

	bool active = true;
	
    public override void OnTriggerEnter2D(Collider2D collider)
	{
		if(active)
		{
			if(collider.gameObject.tag == "player")
			{
				collider.gameObject.GetComponent<PlayerController>().takeDamage();
			}
			else if(collider.gameObject.tag == "surface")
			{
                active = false;
                velocity = Vector2.zero;
				shurikenRigidbody2D.isKinematic = true;
                transform.SetParent(collider.gameObject.transform);

                foreach (Collider2D currCollider in colliders)
				{
					currCollider.enabled = false;
				}
			}
		}
	}
}
