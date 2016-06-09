using UnityEngine;
using System.Collections;

public class ElectricGrenadeController : GrenadeController {

	public float grenadeDuration;
	public CircleCollider2D electricCollider;

	protected void Explode()
	{
		base.Explode();
		grenadeParticleSystem.Play();
		electricCollider.enabled = true;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			collider.gameObject.GetComponent<PlayerController>().takeDamage();
			destroyGrenade();
		}
	}
}
