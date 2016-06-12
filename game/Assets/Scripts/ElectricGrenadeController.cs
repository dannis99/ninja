using UnityEngine;
using System.Collections;

public class ElectricGrenadeController : GrenadeController {

	public float grenadeDuration;
	public CircleCollider2D electricCollider;
	float startGrenadeRadius = .1f;
	float endGrenadeRadius = 1f;
	float t = 0;

	protected void Update()
	{
		base.Update();
		var shape = grenadeParticleSystem.shape;
		if(exploded && shape.radius < endGrenadeRadius)
		{
			shape.radius = Mathf.Lerp(startGrenadeRadius, endGrenadeRadius, t);
			electricCollider.radius = Mathf.Lerp(startGrenadeRadius, endGrenadeRadius, t);
			t += Time.deltaTime*4;
		}
	}

	protected void Explode()
	{
		base.Explode();
		var shape = grenadeParticleSystem.shape;
		shape.radius = startGrenadeRadius;
		grenadeParticleSystem.Play();
		electricCollider.radius = startGrenadeRadius;
		electricCollider.enabled = true;
		grenadeRigidbody.isKinematic = true;
		Invoke("destroyGrenade", grenadeDuration);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag == "player")
		{
			collider.gameObject.GetComponent<PlayerController>().takeDamage();
			Invoke("destroyGrenade", 1);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Explode();
	}
}
