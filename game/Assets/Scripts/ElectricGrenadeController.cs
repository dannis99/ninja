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
		grenadeRigidbody.isKinematic = true;
		Invoke("destroyGrenade", grenadeDuration);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			collider.gameObject.GetComponent<PlayerController>().takeDamage();
			destroyGrenade();
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Explode();
	}
}
